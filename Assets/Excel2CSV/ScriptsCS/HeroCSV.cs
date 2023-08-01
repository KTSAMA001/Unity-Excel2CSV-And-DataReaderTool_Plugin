using System;
using System.Collections.Generic;
using UnityEngine;
namespace CSV_SPACE
{
	public class HeroCSV:CSVBase
	{
		public string ID { get;  set; }
		public string Name { get;  set; }
		public string SKILL { get;  set; }
		public string TestData { get;  set; }
		public string TestData2 { get;  set; }
	}
	public class HeroCSVLoad
	{
		public static HeroCSV herocsv=new HeroCSV();
		static string filePath = "CSV/Hero";
		public static HeroCSV Load(string id)
		{
			var csvTextAsset = Resources.Load<TextAsset>(filePath);
			var csvData = csvTextAsset.text;
			var csvRows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			//判断哪一行的id与key相匹配
			for (int i = 1; i < csvRows.Length; i++)
			{
				var row = csvRows[i].Split(',');
				if (row[0] == id)
				{
					//将这一行赋给heroCSV
				herocsv.ID = row[0];
				herocsv.Name = row[1];
				herocsv.SKILL = row[2];
				herocsv.TestData = row[3];
				herocsv.TestData2 = row[4];
				break;
				}
			}
			return herocsv;
		}
	}
}
