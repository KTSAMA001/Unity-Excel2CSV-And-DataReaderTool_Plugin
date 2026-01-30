using CSV_SPACE;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 改进的使用示例 - 无需反射，类型安全
/// Improved usage examples - No reflection, type-safe
/// </summary>
public class ImprovedUsageExample : MonoBehaviour
{
    void Start()
    {
        // ============================================
        // 方式1: 直接使用生成的类（推荐，无反射）
        // Method 1: Direct usage (Recommended, no reflection)
        // ============================================
        
        // 加载单条数据
        HeroCSV hero = HeroCSVLoad.Load("1");
        if (hero != null)
        {
            Debug.Log($"英雄名称: {hero.Name}, 技能: {hero.SKILL}");
        }
        
        // 获取所有数据
        List<HeroCSV> allHeroes = HeroCSVLoad.GetAll();
        Debug.Log($"总共有 {allHeroes.Count} 个英雄");
        
        // 遍历所有英雄
        foreach (var h in allHeroes)
        {
            Debug.Log($"ID: {h.ID}, 名称: {h.Name}");
        }
        
        // 条件查询 - 查找所有SKILL包含"11"的英雄
        List<HeroCSV> filteredHeroes = HeroCSVLoad.Find(h => h.SKILL.Contains("11"));
        Debug.Log($"找到 {filteredHeroes.Count} 个符合条件的英雄");
        
        // 检查ID是否存在
        if (HeroCSVLoad.Exists("10"))
        {
            Debug.Log("ID为10的英雄存在");
        }
        
        // 获取数据总数
        int count = HeroCSVLoad.Count();
        Debug.Log($"英雄总数: {count}");
        
        // ============================================
        // 方式2: 使用LINQ进行复杂查询
        // Method 2: Complex queries with LINQ
        // ============================================
        
        // 使用LINQ查询
        var heroes = HeroCSVLoad.GetAll();
        
        // 查找名称包含"Alice"的英雄
        var aliceHeroes = heroes.FindAll(h => h.Name.Contains("Alice"));
        
        // 排序
        allHeroes.Sort((a, b) => string.Compare(a.Name, b.Name));
        
        // 使用LINQ
        var firstThree = System.Linq.Enumerable.Take(allHeroes, 3);
        
        // ============================================
        // 方式3: 物品数据示例
        // Method 3: Item data example
        // ============================================
        
        ItemCSV item = ItemCSVLoad.Load("1");
        if (item != null)
        {
            Debug.Log($"物品: {item.CN} ({item.EN}), 效果: {item.Effect}");
        }
        
        // 查找所有特殊物品
        var specialItems = ItemCSVLoad.Find(i => i.ID.StartsWith("11"));
        foreach (var specialItem in specialItems)
        {
            Debug.Log($"特殊物品: {specialItem.CN}");
        }
        
        // ============================================
        // 方式4: 多语言数据使用
        // Method 4: Localization data usage
        // ============================================
        
        // 获取当前语言设置（示例）
        string currentLanguage = "CN"; // 或 "EN"
        
        LanCSV lanData = LanCSVLoad.Load("1");
        if (lanData != null)
        {
            // 根据语言动态获取文本
            string text = currentLanguage == "CN" ? lanData.CN : lanData.EN;
            Debug.Log($"多语言文本: {text}");
        }
        
        // ============================================
        // 性能对比示例
        // Performance comparison
        // ============================================
        
        // 旧方式（使用反射，慢）
        var startTime = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            CSVReader.ReadDataRow("HeroCSV", "1", "Name");
        }
        startTime.Stop();
        Debug.Log($"旧方式（反射）耗时: {startTime.ElapsedMilliseconds}ms");
        
        // 新方式（直接调用，快）
        startTime = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            HeroCSVLoad.Load("1");
        }
        startTime.Stop();
        Debug.Log($"新方式（直接）耗时: {startTime.ElapsedMilliseconds}ms");
    }
    
    // ============================================
    // 高级用法示例
    // Advanced usage examples
    // ============================================
    
    /// <summary>
    /// 创建一个通用的数据管理器（可选）
    /// </summary>
    public class DataManager
    {
        private static DataManager instance;
        public static DataManager Instance => instance ?? (instance = new DataManager());
        
        // 预加载所有数据
        public void PreloadAllData()
        {
            HeroCSVLoad.GetAll();
            ItemCSVLoad.GetAll();
            LanCSVLoad.GetAll();
            Debug.Log("所有CSV数据已预加载到内存");
        }
        
        // 重新加载所有数据
        public void ReloadAllData()
        {
            HeroCSVLoad.Reload();
            ItemCSVLoad.Reload();
            LanCSVLoad.Reload();
            Debug.Log("所有CSV数据已重新加载");
        }
        
        // 获取统计信息
        public void PrintStatistics()
        {
            Debug.Log($"英雄数量: {HeroCSVLoad.Count()}");
            Debug.Log($"物品数量: {ItemCSVLoad.Count()}");
            Debug.Log($"语言条目: {LanCSVLoad.Count()}");
        }
    }
    
    // 示例：使用数据管理器
    void ExampleDataManager()
    {
        // 预加载所有数据
        DataManager.Instance.PreloadAllData();
        
        // 打印统计信息
        DataManager.Instance.PrintStatistics();
        
        // 重新加载数据（例如在更新后）
        DataManager.Instance.ReloadAllData();
    }
}
