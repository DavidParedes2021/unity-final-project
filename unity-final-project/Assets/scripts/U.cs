using UnityEngine;

public class U
{
    public static T GetOrAddComponent<T>(GameObject obj) where T:Component
    {
        return obj.GetComponent<T>() == null ? obj.AddComponent<T>() : obj.GetComponent<T>();
    }

}