using System;
using System.Collections.Generic;
using UnityEngine;
namespace CSV_SPACE
{
	public class LanCSV:CSVBase
	{
		public string ID { get;  set; }
		public string CN { get;  set; }
		public string EN { get;  set; }
	}
	public class LanCSVLoad
	{
		public static LanCSV lancsv=new LanCSV();
		static string filePath = "CSV/Lan";
		public static LanCSV Load(string id)
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
				lancsv.ID = row[0];
				lancsv.CN = row[1];
				lancsv.EN = row[2];
				break;
				}
			}
			return lancsv;
		}
	}
}
