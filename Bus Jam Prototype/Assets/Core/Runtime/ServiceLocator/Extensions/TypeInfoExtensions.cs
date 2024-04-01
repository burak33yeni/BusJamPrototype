using System;
using System.Collections.Generic;
using System.Reflection;
using Core.ServiceLocator;

internal static class TypeInfoExtensions
{
    private const BindingFlags BINDING_FLAGS =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    internal static List<FieldInfo> GetResolvingFields(this Type type)
    {
        List<FieldInfo> resolvingFields = new();
        AddResolvingFields(type, resolvingFields);
        
        type = type.BaseType;
        while(type != null)
        {
            AddResolvingFields(type, resolvingFields);
            type = type.BaseType;
        }
        
        return resolvingFields;
    }

    private static void AddResolvingFields(Type type, List<FieldInfo> resolvingFields)
    {
        FieldInfo[] fields = type.GetFields(BINDING_FLAGS);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo fieldInfo = fields[i];
            if (!fieldInfo.IsDefined(typeof(ResolveAttribute))) continue;
            resolvingFields.Add(fieldInfo);
        }
    }
}