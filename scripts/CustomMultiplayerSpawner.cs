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

        nodeData[PATHKEY] = path;

        foreach (string propertyName in properties)
        {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
              
                var value = property.GetValue(obj);
                nodeData[propertyName] = ConvertToVariant(value,property,true); 
            }
            else
            {
                FieldInfo field = obj.GetType().GetField(propertyName);
                if (field != null)
                {
                    var value = field.GetValue(obj);
                    nodeData[propertyName] = ConvertToVariant(value, field);
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


    public static Variant ConvertToVariant(object value, object property, bool isProperty = false)
    {

        object variableType = isProperty ? ((PropertyInfo)property).PropertyType : ((FieldInfo)property).FieldType;

        if (variableType == typeof(bool))
        {
            return Variant.From<bool>((bool)value);
        }
        if (variableType == typeof(Vector3))
        {
            return Variant.From<Vector3>((Vector3)value);
        }
        if (variableType == typeof(string))
        {
            return Variant.From<string>((string)value);
        }
        if (variableType == typeof(int))
        {
            return Variant.From<int>((int)value);
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
                object value = entry.Value;
                property.SetValue(obj, value);
            }
            else
            {
                FieldInfo field = obj.GetType().GetField(entry.Key.ToString());
                if (field != null)
                {
                    Type fieldType = field.FieldType;
                    object value = entry.Value;
                    field.SetValue(obj, value);
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
