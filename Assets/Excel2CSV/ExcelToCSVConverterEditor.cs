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
        // ѡ��Excel�ļ���
        // string excelFolderPath = EditorUtility.OpenFolderPanel("Select Excel Folder", "", "");

        
        //���excel�ļ��в����ڣ��򴴽�һ����Ȼ���ж��Ƿ�Ϊ�գ�Ϊ���򱨸�
        if (!Directory.Exists(excelFolderPath))
        {
            Directory.CreateDirectory(excelFolderPath);
        }
        if (Directory.GetFiles(excelFolderPath).Length == 0)
        {
            Debug.LogError("Excel�ļ���Ϊ�գ����飡");
            return;
        }
        // ѡ��CSV�ļ���
        //  string csvFolderPath = EditorUtility.OpenFolderPanel("Select CSV Folder", "", "");
        //����ļ��в����ھ������ļ���
        if (!Directory.Exists(csvFolderPath))
        {
            Directory.CreateDirectory(csvFolderPath);
        }
        if (!Directory.Exists(csFolderPath))
        {
            Directory.CreateDirectory(csFolderPath);
        }
        //Unity�༭���ļ�ˢ��
        AssetDatabase.Refresh();
       
        // ����Excel�ļ����е�����Excel�ļ�
        foreach (string excelFilePath in Directory.GetFiles(excelFolderPath, "*.xlsx"))
        {
            // ��ȡCSV�ļ���
            string csvFileName = Path.GetFileNameWithoutExtension(excelFilePath) + ".csv";

            // ƴ��CSV�ļ�·��
            string csvFilePath = Path.Combine(csvFolderPath, csvFileName);
            // ����CSV�ļ���
            var fileName = Path.GetFileName(csvFileName).Replace(".csv", "");
            // ��Excel�ļ�ת��ΪCSV�ļ�
            ExcelToCSV(excelFilePath, csvFilePath);
            //Unity�༭���ļ�ˢ��
            AssetDatabase.Refresh();
            // �ȴ� CSV �ļ��������
            GenerateCSharpScript("CSV" + "/" + fileName, csFolderPath, fileName + "CSV", nameSpaceStr);

        }

        //Unity�༭���ļ�ˢ��
        AssetDatabase.Refresh();

        Debug.Log("Excel files have been converted to CSV successfully.");
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
        using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // ��ȡ�������
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        //Ϊfalse�Ļ������Զ�����������xlsl�ɼ��ĵ�һ�н�����ʵ�ʵĵڶ�������
                        UseHeaderRow = true // ʹ�õ�һ����Ϊ���������Զ�������������xlsl����еĿɼ���һ����Ϊ����
                    }
                });

                var table = dataSet.Tables[0];

                // ��ȡ����//Excel�����������������д��{}�У�������Զ��޳�{}�е�����
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

                // д��CSV�ļ�
                using (var writer = new StreamWriter(csvFilePath))
                {
                    // д���ͷ
                    writer.WriteLine(string.Join(",", columns));

                    // д������
                    foreach (DataRow row in table.Rows)
                    {
                        var fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                        writer.WriteLine(string.Join(",", fields));
                    }
                }
            }
        }

    }

    // ����C#�ű����ڶ�ȡCSV����
    private static void GenerateCSharpScript(string csvFilePath, string csharpOutputFolderPath, string className, string namespaceName)
    {
        // ��ȡCSV�ļ�������
        //var csvData = File.ReadAllText(csvFilePath);
        var csvTextAsset = Resources.Load<TextAsset>(csvFilePath);
     
        Debug.Log($"{csvFilePath}��csvTextAsset:{csvTextAsset}Ϊ�գ�{csvTextAsset==null}");
        var csvData = csvTextAsset.text;
        var csvRows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // ��ȡ������������
        var columnNameList = csvRows[0].Split(',');
        var rowList = new List<string[]>();
        for (int i = 1; i < csvRows.Length; i++)
        {
            var row = csvRows[i].Split(',');
            rowList.Add(row);
        }

        // ����C#��Ĵ���
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
        sb.AppendLine("\t\t\t//�ж���һ�е�id��key��ƥ��");
        sb.AppendLine("\t\t\tfor (int i = 1; i < csvRows.Length; i++)");
        sb.AppendLine("\t\t\t{");
        sb.AppendLine($"\t\t\t\tvar row = csvRows[i].Split(',');");
        sb.AppendLine("\t\t\t\tif (row[0] == id)");
        sb.AppendLine("\t\t\t\t{");
        sb.AppendLine("\t\t\t\t\t//����һ�и���heroCSV");
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

        // ����C#��Ĵ���
        var csharpFilePath = Path.Combine(csharpOutputFolderPath, className + ".cs");
        File.WriteAllText(csharpFilePath, sb.ToString());
    }

}

#endif