using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace DALLE_LIB
{
	public static class ApiKeyEncryptionService
	{
		private static string CreateUserKey()
		{
			var deviceInfo = new string[]
			{
				DeviceInfo.Current.Name,
				DeviceInfo.Current.DeviceType.ToString(),
				DeviceInfo.Current.Manufacturer,
				DeviceInfo.Current.Model,
				DeviceInfo.Current.Platform.ToString()
			};

			var deviceInfoBytes = new List<byte[]>();
			for (int i = 0; i < deviceInfo.Length; i++)
			{
				deviceInfoBytes.Add(Encoding.UTF8.GetBytes(deviceInfo[i]));
			}

			deviceInfoBytes = deviceInfoBytes.OrderByDescending(arr => arr.Length).ToList();

			var resultBytes = new byte[deviceInfoBytes.First().Length];
			resultBytes = deviceInfoBytes[0].ToArray();

			for (int i = 0; i < resultBytes.Length; i++)
			{
				for (int j = 1, k = 0; j < deviceInfoBytes[j].Length; j++, k++)
				{
					if (k == deviceInfoBytes[j].Length)
					{
						k = 0;
					}

					resultBytes[i] ^= deviceInfoBytes[j][k];
				}
			}

			return Encoding.UTF8.GetString(resultBytes);
		}

		private static string EncryptUserKey(string userKey)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(userKey));
		}

		private static string DecryptUserKey(string userKey)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(userKey));
		}

		public static string EncryptApiKey(string apiKey)
		{
			if (apiKey == null || apiKey == "" || apiKey == String.Empty)
			{
				throw new ArgumentNullException("Api key can't be null");
			}

			var userKey = CreateUserKey();
			var encryptUserKey = EncryptUserKey(userKey);

			var encryptApiKey = new byte[apiKey.Length];
			for (int i = 0, k = 0; i < apiKey.Length; i++, k++)
			{
				if (k == encryptUserKey.Length)
				{
					k = 0;
				}

				encryptApiKey[i] = (byte)(apiKey[i] ^ encryptUserKey[k]);
			}

			return Encoding.UTF8.GetString(encryptApiKey);
		}

		public static string DecryptApiKey(string encryptApiKey)
		{
			if (encryptApiKey == null || encryptApiKey == "" || encryptApiKey == String.Empty)
			{
				throw new ArgumentNullException("Api key can't be null");
			}

			var userKey = CreateUserKey();
			var encryptUserKey = EncryptUserKey(userKey);

			var apiKey = new byte[encryptApiKey.Length];
			for (int i = 0, k = 0; i < encryptApiKey.Length; i++, k++)
			{
				if (k == encryptUserKey.Length)
				{
					k = 0;
				}

				apiKey[i] = (byte)(encryptApiKey[i] ^ encryptUserKey[k]);
			}

			return Encoding.UTF8.GetString(apiKey);
		}
	}
}
