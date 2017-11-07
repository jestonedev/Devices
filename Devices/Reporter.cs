using System;
using System.Collections.Generic;
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

		private bool CopyDirectory(string directoryFrom, string directoryTo)
		{
			var files = Directory.GetFiles(directoryFrom);
			var direcotries = Directory.GetDirectories(directoryFrom);
			for (var i = 0; i < files.Length; i++)
			{
				var fileName = Path.GetFileName(files[i]);
				File.Copy(directoryFrom + "\\" + fileName, directoryTo + "\\" + fileName);
			}
			for (var i = 0; i < direcotries.Length; i++)
			{
				var directoryName = Path.GetFileName(direcotries[i]);
				Directory.CreateDirectory(directoryTo + "\\" + directoryName);
				CopyDirectory(directoryFrom + "\\" + directoryName, directoryTo + "\\" + directoryName);
			}
			return true;
		}

		private bool DeleteDirectory(string directoryName)
		{
			Directory.Delete(directoryName, true);
			return true;
		}

		public void DevicesReport(List<Device> devices)
		{
			var rows = "";
			for (var i = 0; i < devices.Count; i++)
			{
				rows += GetOdtDeviceReportRow();
				rows = rows.Replace("${n}", (i+1).ToString());
				rows = rows.Replace("${name}",devices[i].Name);
				rows = rows.Replace("${department}", devices[i].Department);
				rows = rows.Replace("${invenotryNumber}", devices[i].InventoryNumber);
				rows = rows.Replace("${serialNumber}", devices[i].SerialNumber);
			}

			//Скопировать папку отчета во временную папку
			var tempDirecotry = Path.GetTempPath() + Guid.NewGuid();
			Directory.CreateDirectory(tempDirecotry);
			var reportDirecotry = Application.StartupPath + "\\dev_report";
			if (!CopyDirectory(reportDirecotry, tempDirecotry))
				throw new ApplicationException("Не удалось скопировать каталог отчета во временную папку");
			//Произвести замену шаблона
			var xmlPath = tempDirecotry+"\\content.xml";
			var sr = new StreamReader(xmlPath);
			var xml = sr.ReadToEnd();
			xml = xml.Replace("${row}", rows);
			sr.Close();
			var sw = new StreamWriter(xmlPath, false);
			sw.Write(xml);
			sw.Flush();
			sw.Close();
		    var psi = new ProcessStartInfo(Application.StartupPath + "\\7z.exe")
		    {
		        Arguments = "a -tzip -r \"" + tempDirecotry + ".odt\" \"" + tempDirecotry + "\\*\"",
		        UseShellExecute = false,
		        CreateNoWindow = true
		    };
		    var proc = Process.Start(psi);
		    if (proc == null)
		        throw new ApplicationException("Не удалость запустить процесс формирования отчета");
			if (!proc.WaitForExit(20000))
				throw new ApplicationException("Время генерации отчета истекло");
			if (!DeleteDirectory(tempDirecotry))
				throw new ApplicationException("Не удалось очистить временные файлы");
		    var psi2 = new ProcessStartInfo("explorer.exe")
		    {
		        Arguments = "\"" + tempDirecotry + ".odt\"",
		        UseShellExecute = true
		    };
		    Process.Start(psi2);
		}
	}
}
