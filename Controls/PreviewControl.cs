using mangareader.Component;
using mangareader.Utils;

namespace mangareader.Controls
{
	public class BookReadEventArgs : EventArgs
	{
		public List<(string name, byte[] imageData)> imageFileList { get; }
		public FileInfo[] imageFIArray = Array.Empty<FileInfo>();
		public BookReadEventArgs(List<(string name, byte[] imageData)> imageFileList) => this.imageFileList = imageFileList;
		public BookReadEventArgs(FileInfo[] imageFIArray) => this.imageFIArray = imageFIArray;
	}
	public partial class PreviewControl : UserControl
	{
		private readonly AppConfig appConfig;
		//private FileInfo[] imageFiles = Array.Empty<FileInfo>();
		//private List<(string name, byte[] imageData)> imageFilesMem;
		public event EventHandler<BookReadEventArgs>? BookReadEvent;

		//private Button btnRead;

		public PreviewControl()
		{
			appConfig = ConfigManager.LoadSettings();
			var filePath = appConfig.MangaRootDir;
			//btnRead = new Button() {
			//	Text	= "Start Reading",
			//	Dock = DockStyle.Top,
			//	Height = 50,
			//};

			//btnRead.Click += (s, e) => {
					// TODO: add load the image files directly from the ReadControl.
			//	BookReadEvent_click();
			//};

			InitializeComponent();

			//flowLayoutPanel.Controls.Add(btnRead);

			if (Directory.Exists(filePath))
			{
				FileInfo[] imageList = ImageLoader.ExtractImageFromDir(filePath);
				foreach (FileInfo file in imageList)
				{
					Image thumb;
					using var img = Image.FromFile(file.FullName);
					thumb = new Bitmap(img, new Size(150, 200));
					var loopPanel = ThumbnailMaker.Create(thumb, file.Name, (s, e) => BookReadEvent_click(imageList));
					flowLayoutPanel.Controls.Add(loopPanel);
				}
			}
			else if (File.Exists(filePath) && Helper.IsArchive(filePath))
			{
				if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					List<(string name, byte[] imageData)> imageList = ImageLoader.ExtractImageFromZip(filePath);
					//FirstImage(imageFilesMem[0]);
					foreach (var (name, imageData) in imageList)
					{
						Image thumb;
						using (var ms = new MemoryStream(imageData))
						using (var img = Image.FromStream(ms))
						{
							thumb = new Bitmap(img, new Size(150, 200));
						}

						var loopPanel = ThumbnailMaker.Create(thumb, name, (s, e) => BookReadEvent_click(imageList));
						flowLayoutPanel.Controls.Add(loopPanel);
					}
				}
				else if (filePath.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
				{
					// rar handling
					MessageBox.Show("*.rar is not supported yet.");
				}
			}
		}
		private void BookReadEvent_click(FileInfo[] imageList) =>
			BookReadEvent?.Invoke(this, new BookReadEventArgs(imageList));
		private void BookReadEvent_click(List<(string name, byte[] imageData)> imageList) =>
			BookReadEvent?.Invoke(this, new BookReadEventArgs(imageList));
		//private void FirstImage((string name, byte[] imageData) coverMem)
		//{
		//	Image thumb;
		//	using (var ms = new MemoryStream(coverMem.imageData))
		//	using (var img = Image.FromStream(ms))
		//	{
		//		thumb = new Bitmap(img, new Size(150, 200));
		//	}

		//	var thumbPanel = ThumbnailMaker.Create(thumb, coverMem.name, (s, e) => BookReadEvent_click());
		//	flowLayoutPanel.Controls.Add(thumbPanel);
		//}
	}
}
