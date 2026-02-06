using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace CSV_SPACE
{
	public class ItemCSV:CSVBase
	{
		public string ID { get;  set; }
		public string CN { get;  set; }
		public string EN { get;  set; }
		public string Effect { get;  set; }
		public override string GetID() { return ID; }
	}
	public class ItemCSVLoad
	{
		private static readonly string _csvAssetPath = "CSV/Item";
		private static Dictionary<string, ItemCSV> _tbl;
		private static bool _loaded;

		public static ItemCSV Load(string id)
		{
			TryInit();
			ItemCSV v;
			return _tbl.TryGetValue(id, out v) ? v : null;
		}

		public static ItemCSV Load(int id)
		{
			return Load(id.ToString());
		}

		public static List<ItemCSV> GetAll()
		{
			TryInit();
			return new List<ItemCSV>(_tbl.Values);
		}

		public static List<ItemCSV> Find(System.Predicate<ItemCSV> predicate)
		{
			TryInit();
			var all = new List<ItemCSV>(_tbl.Values);
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
			_tbl = new Dictionary<string, ItemCSV>();
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
				if (cells.Length < 4) continue;
				var entry = new ItemCSV();
				entry.ID = cells[0];
				entry.CN = cells[1];
				entry.EN = cells[2];
				entry.Effect = cells[3];
				_tbl[cells[0]] = entry;
			}
			_loaded = true;
		}
	}
}
