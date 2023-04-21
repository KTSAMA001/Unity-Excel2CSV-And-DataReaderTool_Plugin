using System;
using System.Collections.Generic;
using System.IO;
namespace CSV_SPACE
{
	public class HeroCSV:CSVBase
	{
		public static string filePath="Assets/Excel2CSV/CSV/Hero.csv";
		public string ID { get; private set; }
		public string Name { get; private set; }
		public string SKILL { get; private set; }
		public string TestData { get; private set; }
		public string TestData2 { get; private set; }

		 public static Dictionary<string, HeroCSV> Load(string csvFilePath="Assets/Excel2CSV/CSV/Hero.csv")
		{
			var csvData = File.ReadAllText(csvFilePath);
			var csvRows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			 var result = new Dictionary<string, HeroCSV>();
			//列表的第一项数据默认为列标题
			for (int i = 1; i < csvRows.Length; i++)
			{
				var row = csvRows[i].Split(',');
				var obj = new HeroCSV();
				obj.ID = row[0];
				obj.Name = row[1];
				obj.SKILL = row[2];
				obj.TestData = row[3];
				obj.TestData2 = row[4];
				result.Add(obj.ID,obj);
			}
			return result;
		}
	}
}
