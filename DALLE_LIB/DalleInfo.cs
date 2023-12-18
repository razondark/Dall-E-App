using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALLE_LIB
{
	public static class DalleInfo
	{
		/// <summary>
		/// Creates an image given a prompt.
		/// Returns a list of image objects.
		/// </summary>
		public static readonly string createImageLink = "https://api.openai.com/v1/images/generations";

		/// <summary>
		/// Create image edit.
		/// Returns a list of image objects.
		/// </summary>
		public static readonly string editImageLink = "https://api.openai.com/v1/images/edits";

		/// <summary>
		/// Create image variation.
		/// Returns a list of image objects.
		/// </summary>
		public static readonly string variationImageLink = "https://api.openai.com/v1/images/variations";

        public readonly struct ImageSizes
        {
			/// <summary>
			/// small - 256x256, medium - 512x512, large - 1024x1024
			/// </summary>
			public static readonly ImageSizes Dalle2 = new ImageSizes("256x256", "512x512", "1024x1024");

			/// <summary>
			/// small - 1024x1024, medium - 1024x1792, large - 1792x1024
			/// </summary>
			public static readonly ImageSizes Dalle3 = new ImageSizes("1024x1024", "1024x1792", "1792x1024");

			public string Small { get; }
			public string Medium { get; }
			public string Large { get; }

			private ImageSizes(string small, string medium, string large)
			{
				Small = small;
				Medium = medium;
				Large = large;
			}
		}
    }
}
