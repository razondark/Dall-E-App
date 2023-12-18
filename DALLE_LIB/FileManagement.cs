using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALLE_LIB
{
	public static class FileManagement
	{
		public static void CreateApiKeyFile(string apiKey)
		{
			string path = GetKeyFilePath();

			using (var fstream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				byte[] buffer = Encoding.UTF8.GetBytes(ApiKeyEncryptionService.EncryptApiKey(apiKey));
				fstream.Write(buffer, 0, buffer.Length);
			}
		}

		public static string GetKeyFilePath()
		{
			return AppDomain.CurrentDomain.BaseDirectory + "\\key.rd";
		}

		public static string ReadApiKey()
		{
			string apiKey = null;
			string path = GetKeyFilePath();

			if (File.Exists(path))
			{
				using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
				{
					apiKey = reader.ReadToEnd();
				}
			}
			else
			{
				throw new FileNotFoundException();
			}

			return ApiKeyEncryptionService.DecryptApiKey(apiKey);
		}

		public static string GetLinksFilePath()
		{
			return AppDomain.CurrentDomain.BaseDirectory + "\\links.rd";
		}

		public static void CreateLinksFile()
		{
			string path = GetLinksFilePath();

			using (var fstream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{

			}
		}

		public static List<string> ReadLinks()
		{
			string path = GetLinksFilePath();

			if (!File.Exists(path))
			{
				throw new FileNotFoundException(GetLinksFilePath());
			}

			List<string> links = null;

			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
			{
				while (!reader.EndOfStream)
				{
					links.Add(reader.ReadLine());
				}
			}

			return links;
		}
	}
}
