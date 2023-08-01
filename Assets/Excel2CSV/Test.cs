using CSV_SPACE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //csv文件路径
    public string csvFilePath;

    public string idToFindHero = "1";
    public string idToFindLan = "1";

    void Update()
    {
        //每秒执行一次
        if (Time.frameCount % 60 == 0)
        {
            //读取Hero数据
            Debug.Log(CSVReader.ReadDataRow("ItemCSV", idToFindHero, "CN"));
        }
        if (Input.GetMouseButtonDown(0))
        {
            //读取Hero数据
           Debug.Log(CSVReader.ReadDataRow("HeroCSV", idToFindHero, "Name"));
        }
    }
}
