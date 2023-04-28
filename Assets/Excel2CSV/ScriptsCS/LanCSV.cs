using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CSV_SPACE
{
	public class LanCSV:CSVBase
	{
		public static string filePath="CSV/Lan";
		public string ID { get; private set; }
		public string CN { get; private set; }
		public string EN { get; private set; }

		 public static Dictionary<string, LanCSV> Load(string csvFilePath="CSV/Lan")
		{
			 var csvTextAsset = Resources.Load<TextAsset>(csvFilePath);
			 var csvData = csvTextAsset.text;
			 var csvRows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			 var result = new Dictionary<string, LanCSV>();
			//列表的第一项数据默认为列标题
			for (int i = 1; i < csvRows.Length; i++)
			{
				var row = csvRows[i].Split(',');
				var obj = new LanCSV();
				obj.ID = row[0];
				obj.CN = row[1];
				obj.EN = row[2];
				result.Add(obj.ID,obj);
			}
			return result;
		}
	}
}
