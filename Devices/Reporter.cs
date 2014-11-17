using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Devices
{
	class Reporter
	{
		private string GetOdtDeviceReportRow()
		{
			return "<table:table-row>"+
					"<table:table-cell table:style-name=\"Таблица1.A2\" office:value-type=\"string\">"+
						"<text:p text:style-name=\"P2\">${n}</text:p>"+
					"</table:table-cell>"+
					"<table:table-cell table:style-name=\"Таблица1.A2\" office:value-type=\"string\">"+
						"<text:p text:style-name=\"P2\">${name}</text:p>"+
					"</table:table-cell>"+
					"<table:table-cell table:style-name=\"Таблица1.A2\" office:value-type=\"string\">"+
						"<text:p text:style-name=\"P2\">${department}</text:p>"+
					"</table:table-cell>"+
					"<table:table-cell table:style-name=\"Таблица1.A2\" office:value-type=\"string\">"+
						"<text:p text:style-name=\"P2\">${invenotryNumber}</text:p>"+
					"</table:table-cell>"+
					"<table:table-cell table:style-name=\"Таблица1.E2\" office:value-type=\"string\">"+
						"<text:p text:style-name=\"P2\">${serialNumber}</text:p>"+
					"</table:table-cell>"+
				"</table:table-row>";
		}

		private bool CopyDirectory(string DirectoryFrom, string DirectoryTo)
		{
			string[] files = Directory.GetFiles(DirectoryFrom);
			string[] direcotries = Directory.GetDirectories(DirectoryFrom);
			for (int i = 0; i < files.Length; i++)
			{
				string fileName = Path.GetFileName(files[i]);
				File.Copy(DirectoryFrom + "\\" + fileName, DirectoryTo + "\\" + fileName);
			}
			for (int i = 0; i < direcotries.Length; i++)
			{
				string directoryName = Path.GetFileName(direcotries[i]);
				Directory.CreateDirectory(DirectoryTo + "\\" + directoryName);
				CopyDirectory(DirectoryFrom + "\\" + directoryName, DirectoryTo + "\\" + directoryName);
			}
			return true;
		}

		private bool DeleteDirectory(string DirectoryName)
		{
			Directory.Delete(DirectoryName, true);
			return true;
		}

		public void DevicesReport(List<Device> devices)
		{
			string rows = "";
			for (int i = 0; i < devices.Count; i++)
			{
				rows += GetOdtDeviceReportRow();
				rows = rows.Replace("${n}", (i+1).ToString());
				rows = rows.Replace("${name}",devices[i].Name);
				rows = rows.Replace("${department}", devices[i].Department);
				rows = rows.Replace("${invenotryNumber}", devices[i].InventoryNumber);
				rows = rows.Replace("${serialNumber}", devices[i].SerialNumber);
			}

			//Скопировать папку отчета во временную папку
			string TempDirecotry = Path.GetTempPath() + Guid.NewGuid().ToString();
			Directory.CreateDirectory(TempDirecotry);
			string ReportDirecotry = Application.StartupPath + "\\dev_report";
			if (!CopyDirectory(ReportDirecotry, TempDirecotry))
				throw new ApplicationException("Не удалось скопировать каталог отчета во временную папку");
			//Произвести замену шаблона
			string xml_path = TempDirecotry+"\\content.xml";
			StreamReader sr = new StreamReader(xml_path);
			string xml = sr.ReadToEnd();
			xml = xml.Replace("${row}", rows);
			sr.Close();
			StreamWriter sw = new StreamWriter(xml_path, false);
			sw.Write(xml);
			sw.Flush();
			sw.Close();
			ProcessStartInfo psi = new ProcessStartInfo(Application.StartupPath + "\\7z.exe");
			psi.Arguments = "a -tzip -r \""+TempDirecotry+".odt\" \""+TempDirecotry+"\\*\"";
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			Process proc = Process.Start(psi);
			if (!proc.WaitForExit(20000))
				throw new ApplicationException("Время генерации отчета истекло");
			if (!DeleteDirectory(TempDirecotry))
				throw new ApplicationException("Не удалось очистить временные файлы");
			ProcessStartInfo psi2 = new ProcessStartInfo("explorer.exe");
			psi2.Arguments = "\""+TempDirecotry+".odt\"";
			psi2.UseShellExecute = true;
			Process.Start(psi2);
		}
	}
}
