#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using ExcelDataReader;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System;
using System.Text.RegularExpressions;

public class ExcelToCSVConverterEditor : EditorWindow
{
    const string excelFolderPath = "Assets/Excel2CSV/Excel";
    const string csvFolderPath = "Assets/Excel2CSV/Resources/CSV";
    const string csFolderPath = "Assets/Excel2CSV/ScriptsCS"; 
    static string nameSpaceStr = "CSV_SPACE";

    [MenuItem("KT CSV Tools/Convert Excel to CSV", priority = 5)]
    public static void  ConvertExcelToCSV()
    {
        // 选择Excel文件夹
        // string excelFolderPath = EditorUtility.OpenFolderPanel("Select Excel Folder", "", "");

        
        // 如果excel文件夹不存在，就创建一个，然后判断是否为空，为空就报告
        if (!Directory.Exists(excelFolderPath))
        {
            Directory.CreateDirectory(excelFolderPath);
        }
        if (Directory.GetFiles(excelFolderPath).Length == 0)
        {
            Debug.LogError("Excel文件夹为空，请添加！");
            return;
        }
        // 选择CSV文件夹
        //  string csvFolderPath = EditorUtility.OpenFolderPanel("Select CSV Folder", "", "");
        // 如果文件夹不存在就创建文件夹
        if (!Directory.Exists(csvFolderPath))
        {
            Directory.CreateDirectory(csvFolderPath);
        }
        if (!Directory.Exists(csFolderPath))
        {
            Directory.CreateDirectory(csFolderPath);
        }
       
        int successCount = 0;
        int failCount = 0;
        
        // 遍历Excel文件夹中的所有Excel文件
        foreach (string excelFilePath in Directory.GetFiles(excelFolderPath, "*.xlsx"))
        {
            try
            {
                // 获取CSV文件名
                string csvFileName = Path.GetFileNameWithoutExtension(excelFilePath) + ".csv";

                // 拼接CSV文件路径
                string csvFilePath = Path.Combine(csvFolderPath, csvFileName);
                // 获取CSV文件名
                var fileName = Path.GetFileName(csvFileName).Replace(".csv", "");
                
                // 将Excel文件转换为CSV文件
                ExcelToCSV(excelFilePath, csvFilePath);
                
                // 等待 CSV 文件生成后生成脚本
                GenerateCSharpScript("CSV" + "/" + fileName, csFolderPath, fileName + "CSV", nameSpaceStr);
                
                successCount++;
            }
            catch (Exception ex)
            {
                Debug.LogError($"转换文件 {Path.GetFileName(excelFilePath)} 失败: {ex.Message}");
                failCount++;
            }
        }

        // Unity编辑器文件刷新 - 只刷新一次，提高性能
        AssetDatabase.Refresh();

        Debug.Log($"Excel文件转换完成。成功: {successCount}, 失败: {failCount}");
    }

    [MenuItem("KT CSV Tools/Delete All file", priority = 5)]
    public static void DeleteFolderContents()
    {
        if (Directory.Exists(csvFolderPath))
        {
            // ɾ��Ŀ¼�µ������ļ�����Ŀ¼
            Directory.Delete(csvFolderPath, true);
            Debug.Log($"Deleted contents of folder {csvFolderPath}.");
        }
        else
        {
            Debug.LogWarning($"Folder {csvFolderPath} does not exist.");
        }
        if (Directory.Exists(csFolderPath))
        {
            // ɾ��Ŀ¼�µ������ļ�����Ŀ¼
            Directory.Delete(csFolderPath, true);
            Debug.Log($"Deleted contents of folder {csFolderPath}.");
        }
        else
        {
            Debug.LogWarning($"Folder {csFolderPath} does not exist.");
        }
        //Unity�༭���ļ�ˢ��
        AssetDatabase.Refresh();
    }
    public static void ExcelToCSV(string excelFilePath, string csvFilePath)
    {
        try
        {
            using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 获取数据集
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            // 为false的话会自动添加第一行，xlsl可见的第一行就变成实际的第二行了
                            UseHeaderRow = true // 使用第一行作为列名，自动识别表头，所以xlsl表格中的可见第一行作为表头
                        }
                    });

                    var table = dataSet.Tables[0];

                    // 获取列名 // Excel表格中可以在列名后面写备注在{}中，会被自动剔除{}中的内容
                    var columns = table.Columns.Cast<DataColumn>().Select(column =>
                    {
                        var columnName = column.ColumnName;
                        // 移除大括号及其内容
                        var startIndex = columnName.IndexOf('{');
                        var endIndex = columnName.LastIndexOf('}');
                        if (startIndex >= 0 && endIndex >= 0 && startIndex < endIndex)
                        {
                            columnName = columnName.Remove(startIndex, endIndex - startIndex + 1);
                        }
                        // 移除换行符
                        columnName = Regex.Replace(columnName, @"[\r\n]+", "");
                        return columnName;
                    }).ToList();

                    // 写入CSV文件
                    using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
                    {
                        // 写入表头
                        writer.WriteLine(string.Join(",", columns.Select(c => EscapeCsvField(c))));

                        // 写入数据
                        foreach (DataRow row in table.Rows)
                        {
                            var fields = row.ItemArray.Select(field => EscapeCsvField(field?.ToString() ?? "")).ToArray();
                            writer.WriteLine(string.Join(",", fields));
                        }
                    }
                }
            }
        }
        catch (IOException ex)
        {
            throw new IOException($"无法打开或读取文件 {Path.GetFileName(excelFilePath)}。请确保文件未被其他程序占用。详细错误: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"处理文件 {Path.GetFileName(excelFilePath)} 时发生错误: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// 按照RFC 4180标准转义CSV字段
    /// </summary>
    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";
            
        // 如果字段包含逗号、引号、换行符或回车符，需要用引号包裹并转义内部引号
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            // 将字段中的引号替换为两个引号（CSV转义规则）
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }
        return field;
    }

    // 生成C#脚本用于读取CSV数据
    private static void GenerateCSharpScript(string csvFilePath, string csharpOutputFolderPath, string className, string namespaceName)
    {
        // 获取CSV文件内容
        var csvTextAsset = Resources.Load<TextAsset>(csvFilePath);
     
        if (csvTextAsset == null)
        {
            Debug.LogError($"无法加载CSV文件: {csvFilePath}，请确保文件已正确生成。");
            return;
        }
        
        var csvData = csvTextAsset.text;
        var csvRows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (csvRows.Length == 0)
        {
            Debug.LogError($"CSV文件 {csvFilePath} 为空。");
            return;
        }

        // 获取列名和数据行
        var columnNameList = csvRows[0].Split(',');
        
        // 验证列名
        for (int i = 0; i < columnNameList.Length; i++)
        {
            columnNameList[i] = SanitizeIdentifier(columnNameList[i].Trim());
            if (string.IsNullOrEmpty(columnNameList[i]))
            {
                Debug.LogError($"CSV文件 {csvFilePath} 的第 {i + 1} 列列名为空或无效。");
                return;
            }
        }
        
        // 验证类名
        className = SanitizeIdentifier(className);
        if (string.IsNullOrEmpty(className))
        {
            Debug.LogError($"生成的类名无效: {className}");
            return;
        }

        // 生成C#类的代码
        var sb = new StringBuilder();
        sb.AppendLine($"using System;");
        sb.AppendLine($"using System.Collections.Generic;");
        sb.AppendLine($"using UnityEngine;");
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");
        sb.AppendLine($"\tpublic class {className}:CSVBase");
        sb.AppendLine("\t{");
        for (int i = 0; i < columnNameList.Length; i++)
        {
            var columnName = columnNameList[i];
            var fieldName = char.ToUpper(columnName[0]) + columnName.Substring(1);
            sb.AppendLine($"\t\tpublic string {fieldName} {{ get;  set; }}");
        }
        sb.AppendLine("\t}");
        
        sb.AppendLine($"\tpublic class {className}Load");
        sb.AppendLine("\t{");
        sb.AppendLine($"\t\tstatic string filePath = \"CSV/{className.Replace("CSV","")}\";");
        sb.AppendLine($"\t\tprivate static Dictionary<string, {className}> cache = null;");
        sb.AppendLine();
        
        // 生成Load方法 - 按ID加载单条数据（string版本）
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 根据ID加载单条数据");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine($"\t\t/// <param name=\"id\">字符串类型的ID</param>");
        sb.AppendLine($"\t\tpublic static {className} Load(string id)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tEnsureDataLoaded();");
        sb.AppendLine("\t\t\treturn cache.TryGetValue(id, out var result) ? result : null;");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成Load方法 - int重载版本
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 根据ID加载单条数据（整数重载）");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine($"\t\t/// <param name=\"id\">整数类型的ID</param>");
        sb.AppendLine($"\t\tpublic static {className} Load(int id)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\treturn Load(id.ToString());");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成GetAll方法 - 获取所有数据
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 获取所有数据");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine($"\t\tpublic static List<{className}> GetAll()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tEnsureDataLoaded();");
        sb.AppendLine($"\t\t\treturn new List<{className}>(cache.Values);");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成Find方法 - 条件查询
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 根据条件查找数据");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine($"\t\tpublic static List<{className}> Find(System.Predicate<{className}> predicate)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tEnsureDataLoaded();");
        sb.AppendLine($"\t\t\treturn new List<{className}>(cache.Values).FindAll(predicate);");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成Count方法 - 获取数据总数
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 获取数据总数");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine("\t\tpublic static int Count()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tEnsureDataLoaded();");
        sb.AppendLine("\t\t\treturn cache.Count;");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成Exists方法 - 检查ID是否存在（string版本）
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 检查指定ID的数据是否存在");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine($"\t\t/// <param name=\"id\">字符串类型的ID</param>");
        sb.AppendLine("\t\tpublic static bool Exists(string id)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tEnsureDataLoaded();");
        sb.AppendLine("\t\t\treturn cache.ContainsKey(id);");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成Exists方法 - int重载版本
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 检查指定ID的数据是否存在（整数重载）");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine($"\t\t/// <param name=\"id\">整数类型的ID</param>");
        sb.AppendLine("\t\tpublic static bool Exists(int id)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\treturn Exists(id.ToString());");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成Reload方法 - 重新加载数据
        sb.AppendLine($"\t\t/// <summary>");
        sb.AppendLine($"\t\t/// 重新加载数据（清除缓存并重新读取CSV）");
        sb.AppendLine($"\t\t/// </summary>");
        sb.AppendLine("\t\tpublic static void Reload()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tcache = null;");
        sb.AppendLine("\t\t\tEnsureDataLoaded();");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成EnsureDataLoaded私有方法
        sb.AppendLine("\t\tprivate static void EnsureDataLoaded()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tif (cache == null)");
        sb.AppendLine("\t\t\t{");
        sb.AppendLine($"\t\t\t\tcache = new Dictionary<string, {className}>();");
        sb.AppendLine("\t\t\t\tLoadAllData();");
        sb.AppendLine("\t\t\t}");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 生成LoadAllData方法 - 一次性加载所有数据到缓存
        sb.AppendLine("\t\tprivate static void LoadAllData()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tvar csvTextAsset = Resources.Load<TextAsset>(filePath);");
        sb.AppendLine("\t\t\tif (csvTextAsset == null)");
        sb.AppendLine("\t\t\t{");
        sb.AppendLine("\t\t\t\tDebug.LogError($\"无法加载CSV文件: {filePath}\");");
        sb.AppendLine("\t\t\t\treturn;");
        sb.AppendLine("\t\t\t}");
        sb.AppendLine();
        sb.AppendLine("\t\t\tvar csvData = csvTextAsset.text;");
        sb.AppendLine("\t\t\tvar csvRows = csvData.Split(new char[] { '\\r', '\\n' }, StringSplitOptions.RemoveEmptyEntries);");
        sb.AppendLine();
        sb.AppendLine("\t\t\t// 判断哪一行的id与key相匹配");
        sb.AppendLine("\t\t\tfor (int i = 1; i < csvRows.Length; i++)");
        sb.AppendLine("\t\t\t{");
        sb.AppendLine("\t\t\t\tvar row = ParseCsvLine(csvRows[i]);");
        sb.AppendLine("\t\t\t\tif (row.Length < " + columnNameList.Length + ") continue;");
        sb.AppendLine();
        sb.AppendLine($"\t\t\t\tvar data = new {className}();");
        
        // 生成字段赋值
        for (int j = 0; j < columnNameList.Length; j++)
        {
            var columnName = columnNameList[j];
            var fieldName = char.ToUpper(columnName[0]) + columnName.Substring(1);
            sb.AppendLine($"\t\t\t\tdata.{fieldName} = row[{j}];");
        }
        
        sb.AppendLine("\t\t\t\tcache[row[0]] = data;");
        sb.AppendLine("\t\t\t}");
        sb.AppendLine("\t\t}");
        sb.AppendLine();
        
        // 添加CSV解析辅助方法，支持引号包裹的字段
        sb.AppendLine("\t\tprivate static string[] ParseCsvLine(string line)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tvar result = new List<string>();");
        sb.AppendLine("\t\t\tvar field = new StringBuilder();");
        sb.AppendLine("\t\t\tbool inQuotes = false;");
        sb.AppendLine();
        sb.AppendLine("\t\t\tfor (int i = 0; i < line.Length; i++)");
        sb.AppendLine("\t\t\t{");
        sb.AppendLine("\t\t\t\tchar c = line[i];");
        sb.AppendLine();
        sb.AppendLine("\t\t\t\tif (c == '\"')");
        sb.AppendLine("\t\t\t\t{");
        sb.AppendLine("\t\t\t\t\t// 检查是否是转义的引号");
        sb.AppendLine("\t\t\t\t\tif (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')");
        sb.AppendLine("\t\t\t\t\t{");
        sb.AppendLine("\t\t\t\t\t\tfield.Append('\"');");
        sb.AppendLine("\t\t\t\t\t\ti++; // 跳过下一个引号");
        sb.AppendLine("\t\t\t\t\t}");
        sb.AppendLine("\t\t\t\t\telse");
        sb.AppendLine("\t\t\t\t\t{");
        sb.AppendLine("\t\t\t\t\t\tinQuotes = !inQuotes;");
        sb.AppendLine("\t\t\t\t\t}");
        sb.AppendLine("\t\t\t\t}");
        sb.AppendLine("\t\t\t\telse if (c == ',' && !inQuotes)");
        sb.AppendLine("\t\t\t\t{");
        sb.AppendLine("\t\t\t\t\tresult.Add(field.ToString());");
        sb.AppendLine("\t\t\t\t\tfield.Clear();");
        sb.AppendLine("\t\t\t\t}");
        sb.AppendLine("\t\t\t\telse");
        sb.AppendLine("\t\t\t\t{");
        sb.AppendLine("\t\t\t\t\tfield.Append(c);");
        sb.AppendLine("\t\t\t\t}");
        sb.AppendLine("\t\t\t}");
        sb.AppendLine();
        sb.AppendLine("\t\t\tresult.Add(field.ToString());");
        sb.AppendLine("\t\t\treturn result.ToArray();");
        sb.AppendLine("\t\t}");
        
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        // 保存C#类的代码
        var csharpFilePath = Path.Combine(csharpOutputFolderPath, className + ".cs");
        File.WriteAllText(csharpFilePath, sb.ToString(), Encoding.UTF8);
    }
    
    /// <summary>
    /// 清理和验证标识符，确保符合C#命名规则
    /// </summary>
    private static string SanitizeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return "";
            
        // 移除空白字符
        identifier = identifier.Trim();
        
        // 替换不合法的字符为下划线
        var sb = new StringBuilder();
        for (int i = 0; i < identifier.Length; i++)
        {
            char c = identifier[i];
            if (i == 0)
            {
                // 首字符必须是字母或下划线
                if (char.IsLetter(c) || c == '_')
                    sb.Append(c);
                else if (char.IsDigit(c))
                    sb.Append('_').Append(c); // 如果首字符是数字，前面加下划线
                // 其他字符忽略
            }
            else
            {
                // 后续字符可以是字母、数字或下划线
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                // 其他字符忽略
            }
        }
        
        var result = sb.ToString();
        
        // 检查是否是C#保留关键字
        var keywords = new HashSet<string> 
        { 
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this",
            "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
            "using", "virtual", "void", "volatile", "while"
        };
        
        if (keywords.Contains(result.ToLower()))
        {
            result = "_" + result; // 如果是关键字，前面加下划线
        }
        
        return result;
    }

}

#endif