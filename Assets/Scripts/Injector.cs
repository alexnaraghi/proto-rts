using System;
using UnityEngine;
using System.Collections.Generic;

public class Injector
{
    public enum IfNotExist
    {
        AddNew,
        ReturnNull
    }
    
    public const string GO_NAME = "Dependency";
    private static Dictionary<Type, MonoBehaviour> _dict = new Dictionary<Type, MonoBehaviour>();
    private static GameObject _go;
    
    private static GameObject GO
    {
        get
        {
            if (_go == null)
            {
                _go = new GameObject(GO_NAME);
            }

            return _go;
        }
    }
    
    public static T Get<T>(IfNotExist ifNotExist = IfNotExist.AddNew) where T : MonoBehaviour
    {
        if(_dict.ContainsKey(typeof(T)))
        {
            return (T)_dict[typeof(T)];
        }
        else
        {
            T component =  GO.GetComponent<T>();
            if (component == null && ifNotExist == IfNotExist.AddNew)
            {
                component = GO.AddComponent<T>();
            }

            return component;
        }
    }
    
    public static T AddComponent<T>() where T : MonoBehaviour
    {
        var component = GO.AddComponent<T>();
        if(_dict.ContainsKey(typeof(T)))
        {
            //TODO: Remove old component if dupe.
            _dict[typeof(T)] = component;
        }
        else
        {
            _dict.Add(typeof(T), component);
        }
        return component;
    }
}