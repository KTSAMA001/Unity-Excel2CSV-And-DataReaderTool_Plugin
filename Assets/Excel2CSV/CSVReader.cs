
using CSV_SPACE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public static  class CSVReader
{
    /// <summary>
    /// 通用的数据读取方法
    /// </summary>
    /// <param name="typeName">类型名称，例如 HeroCSV（自动生成的脚本类型，Excel表格的名字+CSV）</param>
    /// <param name="id">查找的数据的ID值</param>
    /// <param name="key">查找的数据的字段</param>
    /// <returns></returns>
    public static object ReadDataRow(string typeName,string id,string key)
    {
        typeName = "CSV_SPACE." + typeName;
       Type typeCSV= Type.GetType(typeName);
       Type typeCSVLoad = Type.GetType(typeName+"Load");
        //通过或反射直接调用静态的加载函数，因为整个工具可能用于热更新，所以采用反射的方式
        //如果项目不会使用热更新，可以尝试普通的调用,例如下面的代码
        //Debug.Log(HeroCSVLoad.Load(id).Name);
        object obj = typeCSVLoad.GetMethod("Load").Invoke(null, new object[] { id });
       
     
        if (obj==null)
        {

            Debug.LogError($"{typeName + "Load"}没有找到对应的数据");
            return null;
        }
   
    try
        {
            return typeCSV.GetProperty(key).GetValue(obj);
        }
        catch 
        {
            Debug.LogError($"请检查{typeName}中是否存在{key}字段！");
            return null;
        }

    }
  
}
