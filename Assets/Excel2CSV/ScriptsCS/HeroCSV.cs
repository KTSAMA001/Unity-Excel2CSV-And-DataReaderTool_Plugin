using System;
using System.Collections.Generic;
using System.Text;
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
		public override string GetID() { return ID; }
	}
	public class HeroCSVLoad
	{
		private static readonly string _csvAssetPath = "CSV/Hero";
		private static Dictionary<string, HeroCSV> _tbl;
		private static bool _loaded;

		public static HeroCSV Load(string id)
		{
			TryInit();
			HeroCSV v;
			return _tbl.TryGetValue(id, out v) ? v : null;
		}

		public static HeroCSV Load(int id)
		{
			return Load(id.ToString());
		}

		public static List<HeroCSV> GetAll()
		{
			TryInit();
			return new List<HeroCSV>(_tbl.Values);
		}

		public static List<HeroCSV> Find(System.Predicate<HeroCSV> predicate)
		{
			TryInit();
			var all = new List<HeroCSV>(_tbl.Values);
			return all.FindAll(predicate);
		}

		public static int Count()
		{
			TryInit();
			return _tbl.Count;
		}

		public static bool Exists(string id)
		{
			TryInit();
			return _tbl.ContainsKey(id);
		}

		public static bool Exists(int id)
		{
			return Exists(id.ToString());
		}

		public static void Reload()
		{
			_loaded = false;
			_tbl = null;
			TryInit();
		}

		private static void TryInit()
		{
			if (_loaded) return;
			_tbl = new Dictionary<string, HeroCSV>();
			var asset = Resources.Load<TextAsset>(_csvAssetPath);
			if (asset == null)
			{
				Debug.LogError("CSV文件加载失败: " + _csvAssetPath);
				_loaded = true;
				return;
			}
			var rows = asset.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			for (int ri = 1; ri < rows.Length; ri++)
			{
				var cells = rows[ri].Split(',');
				if (cells.Length < 5) continue;
				var entry = new HeroCSV();
				entry.ID = cells[0];
				entry.Name = cells[1];
				entry.SKILL = cells[2];
				entry.TestData = cells[3];
				entry.TestData2 = cells[4];
				_tbl[cells[0]] = entry;
			}
			_loaded = true;
		}
	}
}
