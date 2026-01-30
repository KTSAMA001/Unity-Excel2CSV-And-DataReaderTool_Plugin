
using CSV_SPACE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class CSVReader
{
    // 反射缓存，提高性能
    private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
    private static Dictionary<string, MethodInfo> methodCache = new Dictionary<string, MethodInfo>();
    private static Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();

    /// <summary>
    /// 通用的数据读取方法
    /// </summary>
    /// <param name="typeName">类型名称，例如：HeroCSV（自动生成的脚本类型，Excel表名称+CSV）</param>
    /// <param name="id">要查找的数据的ID值</param>
    /// <param name="key">要查找的数据的字段</param>
    /// <returns></returns>
    public static object ReadDataRow(string typeName, string id, string key)
    {
        try
        {
            typeName = "CSV_SPACE." + typeName;
            
            // 使用缓存获取类型
            Type typeCSV = GetTypeFromCache(typeName);
            Type typeCSVLoad = GetTypeFromCache(typeName + "Load");
            
            if (typeCSV == null || typeCSVLoad == null)
            {
                Debug.LogError($"无法找到类型 {typeName} 或 {typeName}Load，请确保已生成对应的CSV脚本。");
                return null;
            }
            
            // 使用缓存获取Load方法
            MethodInfo loadMethod = GetMethodFromCache(typeCSVLoad, "Load");
            if (loadMethod == null)
            {
                Debug.LogError($"{typeName}Load 类中没有找到 Load 方法。");
                return null;
            }
            
            // 通过反射调用静态的Load方法
            object obj = loadMethod.Invoke(null, new object[] { id });
            
            if (obj == null)
            {
                Debug.LogWarning($"{typeName}Load.Load(\"{id}\") 返回了 null，可能是ID不存在。");
                return null;
            }
            
            // 使用缓存获取属性
            PropertyInfo property = GetPropertyFromCache(typeCSV, key);
            if (property == null)
            {
                Debug.LogError($"类型 {typeName} 中不存在字段 {key}，请检查CSV表头是否正确。");
                return null;
            }
            
            return property.GetValue(obj);
        }
        catch (Exception ex)
        {
            Debug.LogError($"读取数据失败: 类型={typeName}, ID={id}, 字段={key}\n错误: {ex.Message}\n堆栈: {ex.StackTrace}");
            return null;
        }
    }
    
    /// <summary>
    /// 泛型版本的数据读取方法
    /// </summary>
    public static T ReadDataRow<T>(string typeName, string id, string key)
    {
        object result = ReadDataRow(typeName, id, key);
        if (result == null)
            return default(T);
            
        try
        {
            return (T)result;
        }
        catch (InvalidCastException ex)
        {
            Debug.LogError($"类型转换失败: 无法将 {result.GetType()} 转换为 {typeof(T)}\n错误: {ex.Message}");
            return default(T);
        }
    }
    
    // 缓存辅助方法
    private static Type GetTypeFromCache(string typeName)
    {
        if (!typeCache.TryGetValue(typeName, out Type type))
        {
            type = Type.GetType(typeName);
            if (type != null)
            {
                typeCache[typeName] = type;
            }
        }
        return type;
    }
    
    private static MethodInfo GetMethodFromCache(Type type, string methodName)
    {
        string key = type.FullName + "." + methodName;
        if (!methodCache.TryGetValue(key, out MethodInfo method))
        {
            method = type.GetMethod(methodName);
            if (method != null)
            {
                methodCache[key] = method;
            }
        }
        return method;
    }
    
    private static PropertyInfo GetPropertyFromCache(Type type, string propertyName)
    {
        string key = type.FullName + "." + propertyName;
        if (!propertyCache.TryGetValue(key, out PropertyInfo property))
        {
            property = type.GetProperty(propertyName);
            if (property != null)
            {
                propertyCache[key] = property;
            }
        }
        return property;
    }
    
    /// <summary>
    /// 清除反射缓存（用于重新加载或更新）
    /// </summary>
    public static void ClearCache()
    {
        typeCache.Clear();
        methodCache.Clear();
        propertyCache.Clear();
    }
}
