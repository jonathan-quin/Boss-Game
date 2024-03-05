using Godot;
using Godot.Collections;


using System;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

public partial class Constants : Node
{
	public static long SERVER_HOST_ID = 1;

    private static string endJSON = "ENDJSONSTRING_gjdsfgjlkdsfjlkepowrjogts";

    private static int jsonCount = 0;

   

    

    public static string createName(object obj, params string[] properties)
	{

        Dictionary<string, string> nodeData = new Dictionary<string, string>();

		foreach (string propertyName in properties)
		{
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                object value = property.GetValue(obj);
                nodeData[propertyName] = value.ToString();
            }
            else
            {
                FieldInfo field = obj.GetType().GetField(propertyName);
                if (field != null)
                {
                    object value = field.GetValue(obj);
                    nodeData[propertyName] = value.ToString();
                }
                else
                {
                    throw new ArgumentException($"Property or field '{propertyName}' not found on object type '{obj.GetType().FullName}'.");
                }
            }
        }

		string json_string = JsonSerializer.Serialize(nodeData);
        GD.Print("just made: ", json_string);

        json_string += endJSON + jsonCount.ToString();
        jsonCount += 1;

        json_string = json_string.ReplaceN( "\",\"" , "___");
        json_string = json_string.ReplaceN("\":\"", "____");

        return json_string;
    }

	public static void loadDataFromName(object obj, string name)
	{
        //GD.Print(name);
        if (!name.Contains(endJSON)) return;

        name = name.Substring(0, name.IndexOf(endJSON));

        name = name.ReplaceN("____", "\":\"");
        name = name.ReplaceN("___", "\",\"");
        name = name.ReplaceN("_", "\"");

        
        //GD.Print(name);

        var nodeData = JsonSerializer.Deserialize<Dictionary<string, string>>(name);

        foreach (var entry in nodeData)
        {
            PropertyInfo property = obj.GetType().GetProperty(entry.Key);
            if (property != null)
            {
                Type propertyType = property.PropertyType;
                object value = Convert.ChangeType(entry.Value, propertyType);
                property.SetValue(obj, value);
            }
            else
            {
                FieldInfo field = obj.GetType().GetField(entry.Key);
                if (field != null)
                {
                    Type fieldType = field.FieldType;
                    object value = Convert.ChangeType(entry.Value, fieldType);
                    field.SetValue(obj, value);
                }
                else
                {
                    throw new ArgumentException($"Property or field '{entry.Key}' not found on object type '{obj.GetType().FullName}'.");
                }
            }
        }
    }


}
