using System;
using System.Collections.Generic;
using UnityEngine;
namespace CSV_SPACE
{
	public class ItemCSV:CSVBase
	{
		public string ID { get;  set; }
		public string CN { get;  set; }
		public string EN { get;  set; }
		public string Effect { get;  set; }
	}
	public class ItemCSVLoad
	{
		public static ItemCSV itemcsv=new ItemCSV();
		static string filePath = "CSV/Item";
		public static ItemCSV Load(string id)
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
				itemcsv.ID = row[0];
				itemcsv.CN = row[1];
				itemcsv.EN = row[2];
				itemcsv.Effect = row[3];
				break;
				}
			}
			return itemcsv;
		}
	}
}
