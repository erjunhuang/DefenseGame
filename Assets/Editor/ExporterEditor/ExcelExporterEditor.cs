using ETModel;
using MongoDB.Bson;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public struct CellInfo
{
	public string Type;
	public string Name;
	public string Desc;
}

public class ExcelMD5Info
{
	public Dictionary<string, string> fileMD5 = new Dictionary<string, string>();

	public string Get(string fileName)
	{
		string md5 = "";
		this.fileMD5.TryGetValue(fileName, out md5);
		return md5;
	}

	public void Add(string fileName, string md5)
	{
		this.fileMD5[fileName] = md5;
	}
}

public class ExcelExporterEditor : EditorWindow
{
	[MenuItem("Tools/导出配置")]
	private static void ShowWindow()
	{
		GetWindow(typeof(ExcelExporterEditor));
	}

	private const string ExcelPath = "../Excel";
	private const string ServerConfigPath = "../Config/";

	private bool isClient;

	private ExcelMD5Info md5Info;
	
	// Update is called once per frame
	private void OnGUI()
	{
		try
		{
			const string clientPath = "./Assets/Resources/Config";

			if (GUILayout.Button("导出客户端配置"))
			{
				this.isClient = true;
                ExportAll(clientPath);
                ExportAllClass(@"./Assets/Game/_Scripts/GameModel", "using QGame.Core.Config;\n namespace GameModel\n{\n");
                //ExportAllClass(@"./Assets/Model/Module/Demo/Config", "namespace ETModel\n{\n");
                //ExportAllClass(@"./Assets/Hotfix/Module/Demo/Config", "using ETModel;\n\nnamespace ETHotfix\n{\n");

                Debug.Log("导出客户端配置完成!");
			}

			//if (GUILayout.Button("导出服务端配置"))
			//{
			//	this.isClient = false;
				
			//	ExportAll(ServerConfigPath);
			//	ExportAllClass(@"../Server/Model/Module/Demo/Config", "namespace ETModel\n{\n");
   //             Debug.Log("导出服务端配置完成!");
			//}
        }
		catch (Exception e)
		{
            Debug.LogError(e);
		}
	}
    private void ExportAll(string exportDir)
    {
        string md5File = Path.Combine(ExcelPath, "md5.txt");
        if (!File.Exists(md5File))
        {
            this.md5Info = new ExcelMD5Info();
        }
        else
        {
            this.md5Info = MongoHelper.FromJson<ExcelMD5Info>(File.ReadAllText(md5File));
        }

        foreach (string filePath in Directory.GetFiles(ExcelPath))
        {
            if (Path.GetExtension(filePath) != ".csv")
            {
                continue;
            }
            if (Path.GetFileName(filePath).StartsWith("~"))
            {
                continue;
            }
            string fileName = Path.GetFileName(filePath);
            string oldMD5 = this.md5Info.Get(fileName);
            string md5 = MD5Helper.FileMD5(filePath);
            this.md5Info.Add(fileName, md5);
            if (md5 == oldMD5)
            {
                continue;
            }
            ExportForCsv(filePath, exportDir);
        }

        File.WriteAllText(md5File, this.md5Info.ToJson());

        Debug.Log("所有表导表完成");
        AssetDatabase.Refresh();
    }

    private void ExportAllClass(string exportDir, string csHead)
	{
		foreach (string filePath in Directory.GetFiles(ExcelPath))
		{
			if (Path.GetExtension(filePath) != ".csv")
			{
				continue;
			}
			if (Path.GetFileName(filePath).StartsWith("~"))
			{
				continue;
			}

            ExportClassForCsv(filePath, exportDir, csHead);
            Debug.Log("生成"+Path.GetFileName(filePath)+"类");
		}
		AssetDatabase.Refresh();
	}

    private void ExportClassForCsv(string fileName, string exportDir, string csHead)
    {
        string protoName = Path.GetFileNameWithoutExtension(fileName);

        string exportPath = Path.Combine(exportDir, $"{protoName}.cs");
        using (FileStream txt = new FileStream(exportPath, FileMode.Create))
        using (StreamWriter sw = new StreamWriter(txt, Encoding.UTF8))
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(csHead);

            sb.Append($"\tpublic class {protoName} :ConfigBase\n");
            sb.Append("\t{\n");
            //sb.Append("\t\tpublic int Id { get; set; }\n");

            DataTable dataTable = OpenCSV(fileName, 0, 0, 0, 0, true);
            for (int j = 0; j < dataTable.Columns.Count; ++j)
            {
                string fieldName = dataTable.Rows[0][j].ToString();
                if (fieldName == "Id" || fieldName == "_id")
                {
                    continue;
                }

                string fieldType = dataTable.Rows[1][j].ToString();
                if (fieldType == "" || fieldName == "")
                {
                    continue;
                }

                sb.Append($"\t\tpublic {fieldType} {fieldName};\n");
            }
            sb.Append("\t}\n");
            sb.Append("}\n");
             
            sw.Write(sb.ToString());
        }
    }
	private void ExportForCsv(string fileName, string exportDir)
	{
        DataTable dataTable = OpenCSV(fileName, 0,0,0,0,true);

        string protoName = Path.GetFileNameWithoutExtension(fileName);
        Debug.Log(protoName + "导表开始");
        string exportPath = Path.Combine(exportDir, $"{protoName}.txt");
        using (FileStream txt = new FileStream(exportPath, FileMode.Create))

        using (StreamWriter sw = new StreamWriter(txt, Encoding.UTF8))
        {
            for (int i = 2; i < dataTable.Rows.Count; ++i)
            {
                if (dataTable.Rows[i][0].ToString() == "")
                {
                    continue;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                for (int j = 0; j < dataTable.Columns.Count; ++j)
                {
                    if (dataTable.Rows[1][j].ToString() == "")
                    {
                        continue;
                    }
                    if (j > 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append($"\"{dataTable.Rows[0][j].ToString()}\":{Convert(dataTable.Rows[1][j].ToString(), dataTable.Rows[i][j].ToString())}");
                }
                sb.Append("}");
                sw.WriteLine(sb.ToString());
            }
        }
        Debug.Log(protoName + "导表完成");

 
    }
    /// <summary>
    /// 打开CSV 文件
    /// </summary>
    /// <param name="fileName">文件全名</param>
    /// <param name="firstRow">开始行</param>
    /// <param name="firstColumn">开始列</param>
    /// <param name="getRows">获取多少行</param>
    /// <param name="getColumns">获取多少列</param>
    /// <param name="haveTitleRow">是有标题行</param>
    /// <returns>DataTable</returns>
    public static DataTable OpenCSV(string fullFileName, Int16 firstRow = 0, Int16 firstColumn = 0, Int16 getRows = 0, Int16 getColumns = 0, bool haveTitleRow = true)
    {
        DataTable dt = new DataTable();
        FileStream fs = new FileStream(fullFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GB18030"));
        //记录每次读取的一行记录
        string strLine = "";
        //记录每行记录中的各字段内容
        string[] aryLine;
        //标示列数
        int columnCount = 0;
        //是否已建立了表的字段
        bool bCreateTableColumns = false;
        //第几行
        int iRow = 1;

        //去除无用行
        if (firstRow > 0)
        {
            for (int i = 1; i < firstRow; i++)
            {
                sr.ReadLine();
            }
        }

        // { ",", ".", "!", "?", ";", ":", " " };
        string[] separators = { "," };
        //逐行读取CSV中的数据
        while ((strLine = sr.ReadLine()) != null)
        {
            
            strLine = strLine.Trim();
             
            aryLine = strLine.Split(separators, StringSplitOptions.None);
            if (bCreateTableColumns == false)
            {
                bCreateTableColumns = true;
                columnCount = aryLine.Length;
                //创建列
                for (int i = firstColumn; i < (getColumns == 0 ? columnCount : firstColumn + getColumns); i++)
                {
                    DataColumn dc
                        = new DataColumn(haveTitleRow == true ? aryLine[i] : "COL" + i.ToString());
                    dt.Columns.Add(dc);
                }

                bCreateTableColumns = true;

                if (haveTitleRow == true)
                {
                    continue;
                }
            }


            DataRow dr = dt.NewRow();
            //for (int j = firstColumn; j < (getColumns == 0 ? columnCount : firstColumn + getColumns); j++)
            //{
            //    dr[j - firstColumn] = aryLine[j];
            //}
            for (int j = firstColumn; j < aryLine.Length; j++)
            {
                dr[j - firstColumn] = aryLine[j];
            }
            dt.Rows.Add(dr);

            iRow = iRow + 1;
            if (getRows > 0)
            {
                if (iRow > getRows)
                {
                    break;
                }
            }

        }

        sr.Close();
        fs.Close();
        return dt;
    }

    private void ExportClassForXlsx(string fileName, string exportDir, string csHead)
    {
        XSSFWorkbook xssfWorkbook;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            xssfWorkbook = new XSSFWorkbook(file);
        }

        string protoName = Path.GetFileNameWithoutExtension(fileName);

        string exportPath = Path.Combine(exportDir, $"{protoName}.cs");
        using (FileStream txt = new FileStream(exportPath, FileMode.Create))
        using (StreamWriter sw = new StreamWriter(txt))
        {
            StringBuilder sb = new StringBuilder();
            ISheet sheet = xssfWorkbook.GetSheetAt(0);
            sb.Append(csHead);

            //sb.Append($"\t[Config((int)({GetCellString(sheet, 0, 0)}))]\n");
            //sb.Append($"\tpublic partial class {protoName}Category : ACategory<{protoName}>\n");
            //sb.Append("\t{\n");
            //sb.Append("\t}\n\n");

            //sb.Append($"\tpublic class {protoName}: IConfig\n");
            sb.Append($"\tpublic class {protoName}\n");
            sb.Append("\t{\n");
            sb.Append("\t\tpublic int Id { get; set; }\n");

            int cellCount = sheet.GetRow(0).LastCellNum;

            for (int i = 0; i < cellCount; i++)
            {
                string fieldDesc = GetCellString(sheet, 1, i);

                if (fieldDesc.StartsWith("#"))
                {
                    continue;
                }

                // s开头表示这个字段是服务端专用
                if (fieldDesc.StartsWith("s") && this.isClient)
                {
                    continue;
                }

                string fieldName = GetCellString(sheet, 1, i);

                if (fieldName == "Id" || fieldName == "_id")
                {
                    continue;
                }

                string fieldType = GetCellString(sheet, 2, i);
                if (fieldType == "" || fieldName == "")
                {
                    continue;
                }

                sb.Append($"\t\tpublic {fieldType} {fieldName};\n");
            }

            sb.Append("\t}\n");
            sb.Append("}\n");

            sw.Write(sb.ToString());
        }
    }
    private void ExportForXlsx(string fileName, string exportDir)
    {
        XSSFWorkbook xssfWorkbook;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            xssfWorkbook = new XSSFWorkbook(file);
        }
        string protoName = Path.GetFileNameWithoutExtension(fileName);
        Debug.Log(protoName + "导表开始");
        string exportPath = Path.Combine(exportDir, $"{protoName}.txt");
        using (FileStream txt = new FileStream(exportPath, FileMode.Create))
        using (StreamWriter sw = new StreamWriter(txt))
        {
            for (int i = 0; i < xssfWorkbook.NumberOfSheets; ++i)
            {
                ISheet sheet = xssfWorkbook.GetSheetAt(i);
                ExportSheet(sheet, sw);
            }
        }
        Debug.Log(protoName + "导表完成");
    }
    private void ExportSheet(ISheet sheet, StreamWriter sw)
    {

        int cellCount = sheet.GetRow(0).LastCellNum;
        CellInfo[] cellInfos = new CellInfo[cellCount];

        for (int i = 0; i < cellCount; i++)
        {

            string fieldDesc = GetCellString(sheet, 0, i);
            string fieldName = GetCellString(sheet, 1, i);
            string fieldType = GetCellString(sheet, 2, i);
            cellInfos[i] = new CellInfo() { Name = fieldName, Type = fieldType, Desc = fieldDesc };
        }

        for (int i = 3; i <= sheet.LastRowNum; ++i)
        {
            if (GetCellString(sheet, i, 0) == "")
            {
                continue;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            IRow row = sheet.GetRow(i);
            for (int j = 0; j < cellCount; ++j)
            {
                string desc = cellInfos[j].Desc.ToLower();
                if (desc.StartsWith("#"))
                {
                    continue;
                }

                // s开头表示这个字段是服务端专用
                if (desc.StartsWith("s") && this.isClient)
                {
                    continue;
                }

                // c开头表示这个字段是客户端专用
                if (desc.StartsWith("c") && !this.isClient)
                {
                    continue;
                }

                string fieldValue = GetCellString(row, j);
                if (fieldValue == "")
                {
                    //throw new Exception($"sheet: {sheet.SheetName} 中有空白字段 {i},{j}");
                }

                if (j > 0)
                {
                    sb.Append(",");
                }
                string fieldName = cellInfos[j].Name;

                if (fieldName == "Id" || fieldName == "_id")
                {
                    if (this.isClient)
                    {
                        fieldName = "Id";
                    }
                    else
                    {
                        fieldName = "_id";
                    }
                }

                string fieldType = cellInfos[j].Type;
                sb.Append($"\"{fieldName}\":{Convert(fieldType, fieldValue)}");
            }
            sb.Append("}");
            sw.WriteLine(sb.ToString());
        }
    }

    private static string Convert(string type, string value)
	{
		switch (type)
		{
			case "int[]":
            case "int32[]":
			case "long[]":
                StringBuilder sb = new StringBuilder();
                string[] vs = value.Split(new[] { "|" }, StringSplitOptions.None);
                for (int i = 0; i < vs.Length; ++i)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(vs[i]);
                }
                return $"[{sb}]";
			case "string[]":
				return $"[{value}]";
			case "int":
			case "int32":
			case "int64":
			case "long":
			case "float":
			case "double":
				return value = (value == "") ? "0" : value;
			case "string":
				return $"\"{value}\"";
			default:
                throw new Exception($"不支持此类型: {type}");
		}
	}

	private static string GetCellString(ISheet sheet, int i, int j)
	{
		return sheet.GetRow(i)?.GetCell(j)?.ToString() ?? "";
	}

	private static string GetCellString(IRow row, int i)
	{
		return row?.GetCell(i)?.ToString() ?? "";
	}

	private static string GetCellString(ICell cell)
	{
		return cell?.ToString() ?? "";
	}
}
