using CSV_SPACE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //csv�ļ�·��
    public string csvFilePath;

    public string idToFindHero = "1";
    public string idToFindLan = "1";

    void Update()
    {
        //ÿ��ִ��һ��
        if (Time.frameCount % 60 == 0)
        {
            //��ȡHero����
            Debug.Log(CSVReader.ReadDataRow("ItemCSV", idToFindHero, "CN"));
        }
        if (Input.GetMouseButtonDown(0))
        {
            //��ȡHero����
           Debug.Log(CSVReader.ReadDataRow("HeroCSV", idToFindHero, "Name"));
        }
    }
}
