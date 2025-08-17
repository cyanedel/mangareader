using mangareader.Forms;
using mangareader.Utils;
using System.Data;
using System.IO.Compression;

namespace mangareader.Controls
{
	public class BookReadEventArgs : EventArgs
	{
		public List<(string name, byte[] imageData)> imageFileList { get; }
		public FileInfo[] imageFIArray = Array.Empty<FileInfo>();
		public BookReadEventArgs(List<(string name, byte[] imageData)> imageFileList)
		{
			this.imageFileList = imageFileList;
		}
		public BookReadEventArgs(FileInfo[] imageFIArray)
		{
			this.imageFIArray = imageFIArray;
		}
	}
	public partial class PreviewControl : UserControl
	{
		private readonly AppConfig appConfig;
		private FileInfo[] imageFiles = Array.Empty<FileInfo>();
		private List<(string name, byte[] imageData)> imageFilesMem;
		//public event Action? bookReadEvent;
		public event EventHandler<BookReadEventArgs> BookReadEvent;

		private Button btnRead;

		public PreviewControl()
		{
			appConfig = ConfigManager.LoadSettings();
			var filePath = appConfig.MangaRootDir;
			btnRead = new Button() {
				Text	= "Start Reading",
				Dock = DockStyle.Top,
				Height = 50,
			};

			btnRead.Click += (s, e) => {
				BookReadEvent_click(s, e);
			};

			InitializeComponent();

			flowLayoutPanel.Controls.Add(btnRead);

			if (Directory.Exists(filePath))
			{
				LoadImages(filePath);
			}
			else if (File.Exists(filePath) && Helper.IsArchive(filePath))
			{
				if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					LoadArchivedImages(filePath);
				}
			}

			//InitializeComponent();
		}
		private void BookReadEvent_click(object sender, EventArgs e) {
			if (imageFilesMem != null && imageFilesMem.Count > 0)
			{
				BookReadEvent?.Invoke(this, new BookReadEventArgs(imageFilesMem));
			}
			else
			{
				BookReadEvent?.Invoke(this, new BookReadEventArgs(imageFiles));
			}
		}
		private void LoadImages(string filePath)
		{
			DirectoryInfo d = new(filePath);
			imageFiles = d.GetFiles("*.*", SearchOption.TopDirectoryOnly)
			.Where(f => f.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
									f.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
									f.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
			.OrderBy(f => Helper.ExtractNumber(f.Name))
			.ToArray();

			foreach (FileInfo file in imageFiles)
			{
				Image thumb;
				using (var img = Image.FromFile(file.FullName))
				{
					thumb = new Bitmap(img, new Size(150, 200));
				}

				var panel = new Panel
				{
					Width = 170,
					Height = 240,
					Margin = new Padding(8),
					BorderStyle = BorderStyle.FixedSingle,
				};

				var pictureBox = new PictureBox
				{
					Image = thumb,
					SizeMode = PictureBoxSizeMode.Zoom,
					Location = new Point(5, 5),
					Size = new Size(160, 200)
				};

				var label = new Label
				{
					Text = file.Name,
					Location = new Point(5, 210),
					Width = 160,
					Height = 20,
					TextAlign = ContentAlignment.MiddleCenter
				};

				panel.Controls.Add(pictureBox);
				panel.Controls.Add(label);
				flowLayoutPanel.Controls.Add(panel);
			}
		}
		private void LoadArchivedImages(string filePath)
		{
			if (File.Exists(filePath) && Helper.IsArchive(filePath))
			{
				if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					imageFilesMem = LoadImagesFromZipToMemory(filePath);
					FirstImage(imageFilesMem[1]);
					foreach (var imageItem in imageFilesMem)
					{
						Image thumb;
						using (var ms = new MemoryStream(imageItem.imageData))
						using (var img = Image.FromStream(ms))
						{
							thumb = new Bitmap(img, new Size(150, 200));
						}

						var panel = new Panel
						{
							Width = 170,
							Height = 240,
							Margin = new Padding(8),
							BorderStyle = BorderStyle.FixedSingle,
						};

						var pictureBox = new PictureBox
						{
							Image = thumb,
							SizeMode = PictureBoxSizeMode.Zoom,
							Location = new Point(5, 5),
							Size = new Size(160, 200)
						};

						var label = new Label
						{
							Text = imageItem.name,
							Location = new Point(5, 210),
							Width = 160,
							Height = 20,
							TextAlign = ContentAlignment.MiddleCenter
						};

						panel.Controls.Add(pictureBox);
						panel.Controls.Add(label);
						flowLayoutPanel.Controls.Add(panel);
					}
				}
				//else if (filePath.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
				//{
				//	var imageFiles = LoadFromRar(filePath);
				//	// Pass stream to your image viewer
				//}
			}
		}
		private List<(string name, byte[] imageData)> LoadImagesFromZipToMemory(string zipPath)
		{
			var images = new List<(string, byte[])>();

			using var zip = ZipFile.OpenRead(zipPath);
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
		private void FirstImage((string name, byte[] imageData) coverMem)
		{
			Image thumb;
			using (var ms = new MemoryStream(coverMem.imageData))
			using (var img = Image.FromStream(ms))
			{
				thumb = new Bitmap(img, new Size(150, 200));
			}

			var panel = new Panel
			{
				Width = 170,
				Height = 240,
				Margin = new Padding(8),
				BorderStyle = BorderStyle.FixedSingle,
			};
			//panel.Click += (s, e) => { ParentForm.ToggleView("read"); };

			var pictureBox = new PictureBox
			{
				Image = thumb,
				SizeMode = PictureBoxSizeMode.Zoom,
				Location = new Point(5, 5),
				Size = new Size(160, 200)
			};
			//pictureBox.Click += (s, e) => { ParentForm.ToggleView("read"); };

			var label = new Label
			{
				Text = "Book Title",
				Location = new Point(5, 210),
				Width = 160,
				Height = 20,
				TextAlign = ContentAlignment.MiddleCenter
			};
			//label.Click += (s, e) => { ParentForm.ToggleView("read"); };

			panel.Controls.Add(pictureBox);
			panel.Controls.Add(label);
			flowLayoutPanel.Controls.Add(panel);
		}
	}
}
