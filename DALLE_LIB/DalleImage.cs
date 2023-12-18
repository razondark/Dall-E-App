using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using SkiaSharp;

namespace DALLE_LIB
{
	public class DalleImage : IDisposable
	{
		private class ResponseData
		{
			public class DataItem
			{
				public string url { get; set; }
			}

			public long created { get; set; }
			public List<DataItem> data { get; set; }
		}

		public DalleImage(string apiKey, string user) 
		{
			this.apiKey = apiKey;
			this.user = user;
			this.httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
		}

		public void Dispose()
		{
			this.httpClient.Dispose();
		}

		private HttpClient httpClient;
		private readonly string apiKey;
		private readonly string user;

		public async Task<string> GenerateImageLinkAsync(string message, string size)
		{
			string link = String.Empty;
			//httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

			try
			{
				//if (!httpClient.DefaultRequestHeaders.Any())
				//{
					//httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
				//}

				var requestBody = new
				{
					model = "dall-e-3",
					quality = "standard", // "hd"
					prompt = message,
					n = 1,
					response_format = "url",
					size = size,
					user = user
				};

				string jsonRequest = JsonSerializer.Serialize(requestBody);

				var response = await httpClient.PostAsync(DalleInfo.createImageLink, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);
				}

				var jsonResult = await response.Content.ReadAsStringAsync();
				link = JsonSerializer.Deserialize<ResponseData>(jsonResult).data.Select(l => l.url).First();

				return link;
			}
			catch 
			{
				throw new Exception();
			}
		}

		public async Task<SKImage> GetImageFromLinkAsync(string link)
		{
			SKImage image = null;

			using (var client = new HttpClient())
			{
				var imageStream = await client.GetStreamAsync(link);
				SKBitmap bitmap = SKBitmap.Decode(imageStream);
				image = SKImage.FromBitmap(bitmap);
			}
			return image;
		}

		//public async Task<ImageSource> CompressImageAsync(string link)
		//{
		//	using (var client = new HttpClient())
		//	{
		//		var imageStream = await client.GetStreamAsync(link);
		//		var image = SKImage.FromEncodedData(imageStream);

		//		var imageBitmap = SKBitmap.FromImage(image);
		//		var resizedBitmap = imageBitmap.Resize(new SKImageInfo(imageBitmap.Width / 2, imageBitmap.Height / 2), SKFilterQuality.Medium);

		//		var stream = resizedBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).AsStream();
		//		return ImageSource.FromStream(() => stream);
		//	}
		//}

		//public async Task<Stream> CompressImageAsync(string link)
		//{
		//	using (var client = new HttpClient())
		//	{
		//		var imageStream = await client.GetStreamAsync(link);
		//		SKBitmap bitmap = SKBitmap.Decode(imageStream);

		//		var newWidth = bitmap.Width / 2;
		//		var newHeight = bitmap.Height / 2;

		//		using (var resizedImage = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium)) // or high
		//		{
		//			using (var resizedImageStream = new MemoryStream())
		//			{
		//				resizedImage.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(resizedImageStream);
		//				resizedImageStream.Position = 0;
		//				return resizedImageStream;
		//			}
		//		}
		//	}
		//}

		public void SaveImageToFileAsync(SKImage image)
		{
			if (DeviceInfo.Current.Platform == DevicePlatform.Android)
			{
				//var android = new Android();
				//android

				//string path = Android.App.Application.Current.GetExternalFilesDir(Environment.DirectoryPictures).AbsolutePath;
				//var i = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
			}
			else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
			{

			}
			else
			{
				throw new NotSupportedException("Your device not supported now");
			}
		}
	}
}