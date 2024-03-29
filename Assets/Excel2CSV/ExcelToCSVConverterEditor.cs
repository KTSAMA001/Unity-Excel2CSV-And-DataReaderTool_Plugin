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

        
        //如果excel文件夹不存在，则创建一个，然后判断是否为空，为空则报告
        if (!Directory.Exists(excelFolderPath))
        {
            Directory.CreateDirectory(excelFolderPath);
        }
        if (Directory.GetFiles(excelFolderPath).Length == 0)
        {
            Debug.LogError("Excel文件夹为空，请检查！");
            return;
        }
        // 选择CSV文件夹
        //  string csvFolderPath = EditorUtility.OpenFolderPanel("Select CSV Folder", "", "");
        //如果文件夹不存在就生成文件夹
        if (!Directory.Exists(csvFolderPath))
        {
            Directory.CreateDirectory(csvFolderPath);
        }
        if (!Directory.Exists(csFolderPath))
        {
            Directory.CreateDirectory(csFolderPath);
        }
        //Unity编辑器文件刷新
        AssetDatabase.Refresh();
       
        // 遍历Excel文件夹中的所有Excel文件
        foreach (string excelFilePath in Directory.GetFiles(excelFolderPath, "*.xlsx"))
        {
            // 获取CSV文件名
            string csvFileName = Path.GetFileNameWithoutExtension(excelFilePath) + ".csv";

            // 拼接CSV文件路径
            string csvFilePath = Path.Combine(csvFolderPath, csvFileName);
            // 生成CSV文件名
            var fileName = Path.GetFileName(csvFileName).Replace(".csv", "");
            // 将Excel文件转换为CSV文件
            ExcelToCSV(excelFilePath, csvFilePath);
            //Unity编辑器文件刷新
            AssetDatabase.Refresh();
            // 等待 CSV 文件生成完毕
            GenerateCSharpScript("CSV" + "/" + fileName, csFolderPath, fileName + "CSV", nameSpaceStr);

        }

        //Unity编辑器文件刷新
        AssetDatabase.Refresh();

        Debug.Log("Excel files have been converted to CSV successfully.");
    }

    [MenuItem("KT CSV Tools/Delete All file", priority = 5)]
    public static void DeleteFolderContents()
    {
        if (Directory.Exists(csvFolderPath))
        {
            // 删除目录下的所有文件和子目录
            Directory.Delete(csvFolderPath, true);
            Debug.Log($"Deleted contents of folder {csvFolderPath}.");
        }
        else
        {
            Debug.LogWarning($"Folder {csvFolderPath} does not exist.");
        }
        if (Directory.Exists(csFolderPath))
        {
            // 删除目录下的所有文件和子目录
            Directory.Delete(csFolderPath, true);
            Debug.Log($"Deleted contents of folder {csFolderPath}.");
        }
        else
        {
            Debug.LogWarning($"Folder {csFolderPath} does not exist.");
        }
        //Unity编辑器文件刷新
        AssetDatabase.Refresh();
    }
    public static void ExcelToCSV(string excelFilePath, string csvFilePath)
    {
        using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // 获取表格数据
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        //为false的话，会自动生成列名，xlsl可见的第一行将会变成实际的第二行数据
                        UseHeaderRow = true // 使用第一行作为列名，不自动产生列名，将xlsl表格中的可见第一行作为列名
                    }
                });

                var table = dataSet.Tables[0];

                // 获取列名//Excel表格数据描述可以填写在{}中，下面会自动剔除{}中的数据
                //var columns = table.Columns.Cast<DataColumn>().Select(column => Regex.Replace(column.ColumnName, @"{[^}]*}", "")).ToList();
                var columns = table.Columns.Cast<DataColumn>().Select(column =>
                {
                    var columnName = column.ColumnName;
                    var startIndex = columnName.IndexOf('{');
                    var endIndex = columnName.LastIndexOf('}');
                    if (startIndex >= 0 && endIndex >= 0 && startIndex < endIndex)
                    {
                        columnName = columnName.Remove(startIndex, endIndex - startIndex + 1);
                    }
                    columnName = Regex.Replace(columnName, @"[\r\n]+", "");
                    return columnName;
                }).ToList();

                // 写入CSV文件
                using (var writer = new StreamWriter(csvFilePath))
                {
                    // 写入表头
                    writer.WriteLine(string.Join(",", columns));

                    // 写入数据
                    foreach (DataRow row in table.Rows)
                    {
                        var fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                        writer.WriteLine(string.Join(",", fields));
                    }
                }
            }
        }

    }

    // 生成C#脚本用于读取CSV数据
    private static void GenerateCSharpScript(string csvFilePath, string csharpOutputFolderPath, string className, string namespaceName)
    {
        // 读取CSV文件的数据
        //var csvData = File.ReadAllText(csvFilePath);
        var csvTextAsset = Resources.Load<TextAsset>(csvFilePath);
     
        Debug.Log($"{csvFilePath}：csvTextAsset:{csvTextAsset}为空：{csvTextAsset==null}");
        var csvData = csvTextAsset.text;
        var csvRows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 提取列名和行数据
        var columnNameList = csvRows[0].Split(',');
        var rowList = new List<string[]>();
        for (int i = 1; i < csvRows.Length; i++)
        {
            var row = csvRows[i].Split(',');
            rowList.Add(row);
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
        sb.AppendLine($"\t\tpublic static {className} {className.ToLower()}=new {className}();");
        sb.AppendLine($"\t\tstatic string filePath = \"CSV/{className.Replace("CSV","")}\";");
        sb.AppendLine($"\t\tpublic static {className} Load(string id)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tvar csvTextAsset = Resources.Load<TextAsset>(filePath);");
        sb.AppendLine("\t\t\tvar csvData = csvTextAsset.text;");
        sb.AppendLine("\t\t\tvar csvRows = csvData.Split(new char[] { '\\r', '\\n' }, StringSplitOptions.RemoveEmptyEntries);");
        sb.AppendLine("\t\t\t//判断哪一行的id与key相匹配");
        sb.AppendLine("\t\t\tfor (int i = 1; i < csvRows.Length; i++)");
        sb.AppendLine("\t\t\t{");
        sb.AppendLine($"\t\t\t\tvar row = csvRows[i].Split(',');");
        sb.AppendLine("\t\t\t\tif (row[0] == id)");
        sb.AppendLine("\t\t\t\t{");
        sb.AppendLine("\t\t\t\t\t//将这一行赋给heroCSV");
        for (int j = 0; j < columnNameList.Length; j++)
        {
            var columnName = columnNameList[j];
            var fieldName = char.ToUpper(columnName[0]) + columnName.Substring(1);
            sb.AppendLine($"\t\t\t\t{className.ToLower()}.{fieldName} = row[{j}];");
        }
        sb.AppendLine("\t\t\t\tbreak;");
        sb.AppendLine("\t\t\t\t}");
        sb.AppendLine("\t\t\t}");
        sb.AppendLine($"\t\t\treturn {className.ToLower()};");
        sb.AppendLine("\t\t}");

        sb.AppendLine("\t}");
        sb.AppendLine("}");

        // 保存C#类的代码
        var csharpFilePath = Path.Combine(csharpOutputFolderPath, className + ".cs");
        File.WriteAllText(csharpFilePath, sb.ToString());
    }

}

#endif