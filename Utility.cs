using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public static class Utility
{


    public static T GetValue<T>(this JObject jObj, string key, T defaultValue = default) where T : IComparable, IConvertible, IEquatable<T>
    {
        try
        {
            if (jObj.TryGetValue(key, out JToken token))
                return token.ToObject<T>();
        }
        catch
        {

        }

        return defaultValue;
    }

    public static T GetDeserializedObject<T>(this JObject jObj, string key, T defaultValue = default)
    {
        if (jObj[key] == null)
        {
            return defaultValue;
        }

        return JsonConvert.DeserializeObject<T>(jObj[key].ToString());
    }

    public static void RemoveChildAll(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child != null)
                GameObject.Destroy(child.gameObject);
        }
    }
}
