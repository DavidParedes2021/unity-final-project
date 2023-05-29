using System.Collections.Generic;
using UnityEngine;

public class U
{
    public static T GetOrAddComponent<T>(GameObject obj) where T:Component
    {
        return obj.GetComponent<T>() == null ? obj.AddComponent<T>() : obj.GetComponent<T>();
    }
    public static T RandomElement<T>(List<T> list)
    {
       return list[Random.Range(0, list.Count)];
    }

}