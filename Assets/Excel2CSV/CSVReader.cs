
using CSV_SPACE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public static  class CSVReader
{
    /// <summary>
    /// ͨ�õ����ݶ�ȡ����
    /// </summary>
    /// <param name="typeName">�������ƣ����� HeroCSV���Զ����ɵĽű����ͣ�Excel��������+CSV��</param>
    /// <param name="id">���ҵ����ݵ�IDֵ</param>
    /// <param name="key">���ҵ����ݵ��ֶ�</param>
    /// <returns></returns>
    public static object ReadDataRow(string typeName,string id,string key)
    {
        typeName = "CSV_SPACE." + typeName;
       Type typeCSV= Type.GetType(typeName);
       Type typeCSVLoad = Type.GetType(typeName+"Load");
        //ͨ������ֱ�ӵ��þ�̬�ļ��غ�������Ϊ�������߿��������ȸ��£����Բ��÷���ķ�ʽ
        //�����Ŀ����ʹ���ȸ��£����Գ�����ͨ�ĵ���,��������Ĵ���
        //Debug.Log(HeroCSVLoad.Load(id).Name);
        object obj = typeCSVLoad.GetMethod("Load").Invoke(null, new object[] { id });
       
     
        if (obj==null)
        {

            Debug.LogError($"{typeName + "Load"}û���ҵ���Ӧ������");
            return null;
        }
   
    try
        {
            return typeCSV.GetProperty(key).GetValue(obj);
        }
        catch 
        {
            Debug.LogError($"����{typeName}���Ƿ����{key}�ֶΣ�");
            return null;
        }

    }
  
}
