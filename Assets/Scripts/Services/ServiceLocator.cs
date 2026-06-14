using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();
    
   
    public static void Register<T>(T service)
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            Debug.LogWarning("Service already registered: " + type.Name);
            services[type] = service;
        }
        else
        {
            services.Add(type, service);
            Debug.Log("Service registered: " + type.Name);
        }
    }
    
    
    public static T Get<T>()
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            return (T)services[type];
        }
        
        Debug.LogError("Service not found: " + type.Name);
        return default(T);
    }
    
    public static bool Has<T>()
    {
        return services.ContainsKey(typeof(T));
    }
    
    
    public static void Clear()
    {
        services.Clear();
    }
}
