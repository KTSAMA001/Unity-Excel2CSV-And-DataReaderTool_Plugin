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
            /*      try
                      {
                          //ID为idToFind，输出Hero以及Lan的数据
                          //输出分割线
                          Debug.Log("------------------Hero Data------------------------");
                          Debug.Log(HeroCSV.Load()[idToFindHero].ID);
                          Debug.Log(HeroCSV.Load()[idToFindHero].Name);
                          Debug.Log(HeroCSV.Load()[idToFindHero].SKILL);
                          Debug.Log("------------------Lan Data------------------------");
                          Debug.Log(LanCSV.Load()[idToFindLan].ID);
                          Debug.Log(LanCSV.Load()[idToFindLan].CN);
                          Debug.Log(LanCSV.Load()[idToFindLan].EN);

                      }
                      catch (Exception)
                      {
                          //警告日志 没有正常执行数据输出
                          Debug.LogWarning("没有正常执行数据输出");

                      }*/
        }
        if (Input.GetMouseButtonDown(0))
        {
            //读取Hero数据
        Debug.Log(HeroCSV.Load()[idToFindHero].Name);
        }
    }
}
