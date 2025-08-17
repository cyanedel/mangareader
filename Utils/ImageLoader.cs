using System.IO.Compression;

namespace mangareader.Utils
{
	static class ImageLoader
	{
		public static FileInfo[] ExtractImageFromDir(string filePath)
		{
			DirectoryInfo d = new(filePath);
			var imageFiles = d.GetFiles("*.*", SearchOption.TopDirectoryOnly)
			.Where(f => f.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
									f.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
									f.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
			.OrderBy(f => Helper.ExtractNumber(f.Name))
			.ToArray();

			return imageFiles;
		}
		public static List<(string name, byte[] imageData)> ExtractImageFromZip(string filePath)
		{
			var images = new List<(string, byte[])>();

			using var zip = ZipFile.OpenRead(filePath);
			foreach (var entry in zip.Entries
			.Where(e => !string.IsNullOrEmpty(e.Name) &&
									(e.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
										e.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
										e.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
			.OrderBy(e => Helper.ExtractNumber(e.FullName)))
			{
				byte[] imageData;
				using (var entryStream = entry.Open())
				using (var ms = new MemoryStream())
				{
					entryStream.CopyTo(ms);
					imageData = ms.ToArray();
				}
				images.Add((entry.FullName, imageData));
			}
			return images;
		}
	}
}
