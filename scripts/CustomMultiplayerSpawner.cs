using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class CustomMultiplayerSpawner : MultiplayerSpawner
{
    public override void _EnterTree()
    {
        Globals.multiplayerSpawner = this;
		Callable newSpawnFunction = Callable.From<Godot.Collections.Dictionary,Node>( loadDataFromName);
		SpawnFunction = newSpawnFunction;
    }
	

  private const string PATHKEY = "PATHKEY";

  public static Variant createSpawnRequest(object obj,string path, params string[] properties)
	{

        var nodeData = new Godot.Collections.Dictionary();

        //Dictionary<string, object> nodeData = new Dictionary<string, object>();

        GD.Print("STARTING");

        nodeData[PATHKEY] = path;

        foreach (string propertyName in properties)
        {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
              
                var value = property.GetValue(obj);
                nodeData[propertyName] = ToVariant(value,property,true); 
            }
            else
            {
                FieldInfo field = obj.GetType().GetField(propertyName);
                if (field != null)
                {
                    var value = field.GetValue(obj);
                    nodeData[propertyName] = ToVariant(value,field,false);
                }
                else
                {
                    throw new ArgumentException($"Property or field '{propertyName}' not found on object type '{obj.GetType().FullName}'.");
                }
            }
        }

        GD.Print(nodeData);

        return nodeData;
    }

    private static readonly Dictionary<Type, (Func<object, Variant> ToVariant, Func<Variant, object> FromVariant)> TypeMap = new Dictionary<Type, (Func<object, Variant>, Func<Variant, object>)>()
    {
        { typeof(bool), (value => Variant.From<bool>((bool)value), variant => variant.AsBool()) },
        { typeof(Vector3), (value => Variant.From<Vector3>((Vector3)value), variant => variant.AsVector3()) },
        { typeof(string), (value => Variant.From<string>((string)value), variant => variant.AsString()) },
        { typeof(int), (value => Variant.From<int>((int)value), variant => variant.AsInt32()) },
        { typeof(Transform3D), (value => Variant.From<Transform3D>((Transform3D)value), variant => variant.AsTransform3D()) }
        
    };

    public static Variant ToVariant(object value, object field,bool isProperty)
    {
        Type targetType =  !isProperty ? ((FieldInfo) field).FieldType : ((PropertyInfo) field).PropertyType;

        GD.Print(targetType);

        if (TypeMap.ContainsKey(targetType))
        {
            GD.Print("Yes we have that");
            return TypeMap[targetType].ToVariant(value);
        }

        GD.Print(value);

        throw new ArgumentException();
    }

    public static object FromVariant(Variant variant,object field,bool isProperty)
    {


        Type targetType =  !isProperty ? ((FieldInfo) field).FieldType : ((PropertyInfo) field).PropertyType;

        if (TypeMap.TryGetValue(targetType, out var conversionFuncs))
        {
            return conversionFuncs.FromVariant(variant);
        }

        throw new ArgumentException();
    }

	public static Node loadDataFromName(Godot.Collections.Dictionary nodeData)
	{
        GD.Print("loading data!");


        Node obj = GD.Load<PackedScene>(nodeData[PATHKEY].ToString()).Instantiate();

        GD.Print("node ",obj);

        nodeData.Remove(PATHKEY);

        GD.Print(nodeData);

        foreach (var entry in nodeData)
        {
            PropertyInfo property = obj.GetType().GetProperty(entry.Key.ToString());
            if (property != null)
            {
                Type propertyType = property.PropertyType;
                Variant value = entry.Value;
                property.SetValue(obj, FromVariant(value,property,true) );
            }
            else
            {
                FieldInfo field = obj.GetType().GetField(entry.Key.ToString());
                if (field != null)
                {
                    Type fieldType = field.FieldType;
                    Variant value = entry.Value;
                    field.SetValue(obj, FromVariant(value,field,false));
                }
                else
                {
                    throw new ArgumentException($"Property or field '{entry.Key}' not found on object type '{obj.GetType().FullName}'.");
                }
            }
        }

        GD.Print("all that worked");

        return obj;
    }

}
