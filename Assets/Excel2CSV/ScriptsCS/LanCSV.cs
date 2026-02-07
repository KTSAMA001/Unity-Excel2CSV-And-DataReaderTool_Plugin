using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace CSV_SPACE
{
	public class LanCSV:CSVBase
	{
		public string ID { get;  set; }
		public string CN { get;  set; }
		public string EN { get;  set; }
		public override string GetID() { return ID; }
	}
	public class LanCSVLoad
	{
		private static readonly string _csvAssetPath = "CSV/Lan";
		private static Dictionary<string, LanCSV> _tbl;
		private static bool _loaded;

		public static LanCSV Load(string id)
		{
			TryInit();
			LanCSV v;
			return _tbl.TryGetValue(id, out v) ? v : null;
		}

		public static LanCSV Load(int id)
		{
			return Load(id.ToString());
		}

		public static List<LanCSV> GetAll()
		{
			TryInit();
			return new List<LanCSV>(_tbl.Values);
		}

		public static List<LanCSV> Find(System.Predicate<LanCSV> predicate)
		{
			TryInit();
			var all = new List<LanCSV>(_tbl.Values);
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
			_tbl = new Dictionary<string, LanCSV>();
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
				if (cells.Length < 3) continue;
				var entry = new LanCSV();
				entry.ID = cells[0];
				entry.CN = cells[1];
				entry.EN = cells[2];
				_tbl[cells[0]] = entry;
			}
			_loaded = true;
		}
	}
}
