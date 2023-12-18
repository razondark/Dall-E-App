using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using DALLE_LIB;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace DALLE_UI
{
	public partial class MainPage : ContentPage
	{
		private DalleImage dalleImage;

		private string apiKey { get; set; }

		public MainPage()
		{
			InitializeComponent();

			if (File.Exists(FileManagement.GetKeyFilePath()))
			{
				//this.apiKey = ReadApiKey(); //= "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";
				//this.dalleImage = new DalleImage(this.apiKey, DeviceInfo.Name);
				Init();
			}
			// else -> MainPage_Loaded
		}

		private void Init()
		{
			this.apiKey = FileManagement.ReadApiKey();
			this.dalleImage = new DalleImage(this.apiKey, DeviceInfo.Name);
		}

		private bool IsInternetConnected()
		{
			if (Connectivity.NetworkAccess == NetworkAccess.Internet)
			{
				return true;
			}

			return false;
		}

		private async void ButtonGenerateImage_Clicked(object sender, EventArgs e)
		{
			if (inputRequest.Text == "" || inputRequest.Text == null || inputRequest.Text == String.Empty)
			{
				await Application.Current.MainPage.DisplayAlert("Внимание", "Введите запрос для генерации изображения", "OK");
				return;
			}

			inputRequest.Unfocus();
			var currentContent = Content;

			ActivityIndicator activityIndicator = new ActivityIndicator { IsRunning = true, Color = Colors.DarkBlue };
			Content = activityIndicator;

			try
			{
				var imageLink = await Task.Run(() => dalleImage.GenerateImageLinkAsync(inputRequest.Text, DalleInfo.ImageSizes.Dalle3.Large));

				if (imageLink == null)
				{
					activityIndicator.IsRunning = false;
					Content = currentContent;
					return;
				}

				generatedImage.Source = null; // ??
				//generatedImage.Source = ImageSource.FromUri(new Uri(imageLink));

				
				generatedImage.Source = imageLink;

				// don't work
				//generatedImage.Source = ImageSource.FromStream(() =>
				//{
				//	var task = Task.Run(async () => await dalleImage.CompressImageAsync(imageLink));
				//	return task.Result;
				//});

				while (generatedImage.IsLoading)
				{
					await Task.Delay(100);
				}
			}

			catch (HttpRequestException ex)
			{
				await Application.Current.MainPage.DisplayAlert("Ошибка", "Error " + ex.StatusCode + ": " + ex.Message, "OK");
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
			}
			finally
			{
				activityIndicator.IsRunning = false;
				Content = currentContent;
			}
		}

		private async void ButtonSaveImage_Clicked(object sender, EventArgs e)
		{
			//CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			//string text = "This is a save";
			//ToastDuration duration = ToastDuration.Short;
			//double fontSize = 14;
			//var toast = Toast.Make(text, duration, fontSize);
			//await toast.Show(cancellationTokenSource.Token);

			//var display = DeviceDisplay.MainDisplayInfo;
		}

		private async void MainPage_Loaded(object sender, EventArgs e)
		{

			if (IsInternetConnected() == false)
			{
				await Application.Current.MainPage.DisplayAlert("Внимание", "Для работы приложения необходимо подключение к интернету", "OK");
			}

			if (!File.Exists(FileManagement.GetKeyFilePath()))
			{
				var api = await Application.Current.MainPage.DisplayPromptAsync("Начало", "Для начала работы с программой введите ключ:", "OK", "Отмена");

				if (api == null || api == String.Empty || api == "")
				{
					await Application.Current.MainPage.DisplayAlert("Внимание", "Для работы приложения необходимо ввести ключ", "OK");
					return;
				}
				else if (!Regex.IsMatch(api, @"^[a-zA-Z0-9-]+$"))
				{
					//await Application.Current.MainPage.DisplayAlert("Ошибка", "Ключ может состоять только из английских букв, цифр и специальных символов", "OK");

					while (!Regex.IsMatch(api, @"^[a-zA-Z0-9-]+$") || api == "")
					{
						await Application.Current.MainPage.DisplayAlert("Ошибка", "Ключ может состоять только из английских букв, цифр и специальных символов", "OK");
						api = await Application.Current.MainPage.DisplayPromptAsync("Начало", "Для начала работы с программой введите ключ:", "OK", "Отмена");
					}
				}

				FileManagement.CreateApiKeyFile(api);
				FileManagement.CreateLinksFile();
				Init();

				if (!this.apiKey.Equals(api))
				{
					await Application.Current.MainPage.DisplayAlert("Error", "Error with creating api key file\n" +
																	"Check your api key", "OK");
					return;
				}
			}
			
		}
	}
}
