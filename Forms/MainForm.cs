using mangareader.Controls;
using mangareader.Utils;
using System.IO.Compression;

namespace mangareader.Forms
{
	public class MainForm : Form
	{
		private readonly AppConfig appConfig;
		private FileInfo[] imageFiles = Array.Empty<FileInfo>();
		private List<(string name, byte[] imageData)> imageFilesMem;
		private readonly FlowLayoutPanel flowPanel;
		private readonly Panel panelReadLists;
		private readonly Panel panelThumbnails;
		private readonly ReadControl formRead;
		private readonly PreviewControl previewControl;
		private readonly Button btnRead;

		// Functions /////////////////////
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
				flowPanel.Controls.Add(panel);
			}
		}

		private void LoadArchivedImages(string filePath) {
			if (File.Exists(filePath) && Helper.IsArchive(filePath))
			{
				if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					imageFilesMem = LoadImagesFromZipToMemory(filePath);
					FirstImage(imageFilesMem[0]);
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
            flowPanel.Controls.Add(panel);
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
			panel.Click += (s, e) => { ToggleView("read"); };

			var pictureBox = new PictureBox
			{
				Image = thumb,
				SizeMode = PictureBoxSizeMode.Zoom,
				Location = new Point(5, 5),
				Size = new Size(160, 200)
			};
			pictureBox.Click += (s, e) => { ToggleView("read"); };

			var label = new Label
			{
				Text = "Book Title",
				Location = new Point(5, 210),
				Width = 160,
				Height = 20,
				TextAlign = ContentAlignment.MiddleCenter
			};
			label.Click += (s, e) => { ToggleView("read"); };

			panel.Controls.Add(pictureBox);
			panel.Controls.Add(label);
			flowPanel.Controls.Add(panel);
		}

		// Views /////////////////////
		public MainForm()
		{
			Text = "Manga Reader";
			Size = new Size(1000, 700);
			appConfig = ConfigManager.LoadSettings();

			panelThumbnails = new Panel();
			btnRead = new Button();
			flowPanel = new FlowLayoutPanel();
			formRead = new ReadControl();
			previewControl = new PreviewControl() {
				Dock = DockStyle.Fill,
			};
			panelReadLists = new Panel() {
				Dock = DockStyle.Top,
				Height = 40
			};
			previewControl.BookReadEvent += (s, e) => {
				if (e.imageFileList is List<(string name, byte[] imageData)>) {
					this.imageFilesMem = e.imageFileList;
					formRead.LoadImages(this.imageFilesMem);
					ToggleView("read");
				} else if (e.imageFIArray is FileInfo[]) {
					this.imageFiles = e.imageFIArray;
					formRead.LoadImages(this.imageFiles);
					ToggleView("read");
				}
			};

			InitPanelThumbnails();
			InitPanelRead();
			InitPanelReadList();
			Controls.Add(previewControl);
			Controls.Add(panelThumbnails);

			var filePath = appConfig.MangaRootDir;
			if (Directory.Exists(filePath))
			{
				LoadImages(filePath);
			}
			else if (File.Exists(filePath) && Helper.IsArchive(filePath))
			{
				if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					LoadArchivedImages(filePath);
					//imageFilesMem = LoadImagesFromZipToMemory(filePath);
					// Pass stream to your image viewer
				}

			}
				//ShowImages(appConfig.MangaRootDir);

			ToggleView("");
		}
		public void ToggleView(string state)
		{
			switch (state)
			{
				case "read":
					panelThumbnails.Visible = false;
					formRead.Visible = true;
					formRead.Focus();
					Text = "Manga Reader - Read";
					break;
				default:
					panelThumbnails.Visible = true;
					formRead.Visible = false;
					Text = "Manga Reader - List";
					break;
			}
		}

		private void InitPanelThumbnails()
		{
			panelThumbnails.Dock = DockStyle.Fill;
			panelThumbnails.Visible = false;

			btnRead.Text = "Read";
			btnRead.Dock = DockStyle.Top;
			btnRead.Height = 40;
			btnRead.Click += (s, e) => { ToggleView("read"); };

			flowPanel.Dock = DockStyle.Fill;
			flowPanel.AutoScroll = true;
			flowPanel.WrapContents = true;
			flowPanel.FlowDirection = FlowDirection.LeftToRight;
			flowPanel.Padding = new Padding(10);
				
			panelThumbnails.Controls.Add(flowPanel);
			panelThumbnails.Controls.Add(btnRead);
		}

		private void InitPanelRead()
		{
			formRead.BackReq += () => ToggleView("");
			Controls.Add(formRead);
		}
		private void InitPanelReadList()
		{
			string[] buttonLabels = { "Button 1", "Button 2", "Button 3" };
			int buttonWidth = panelReadLists.ClientSize.Width / buttonLabels.Length;

			for (int i = 0; i < buttonLabels.Length; i++)
			{
				Button btn = new Button();
				btn.Text = buttonLabels[i];
				btn.Height = panelReadLists.Height;
				btn.Width = buttonWidth;
				btn.Left = i * buttonWidth;
				btn.Top = 0;

				// Example click event
				btn.Click += (sender, e) =>
				{
					MessageBox.Show($"{((Button)sender).Text} clicked!");
				};

				// Add to form
				panelReadLists.Controls.Add(btn);
			}
			panelThumbnails.Controls.Add(panelReadLists);
		}
	}
}