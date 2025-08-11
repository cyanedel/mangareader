using System.Diagnostics;

namespace mangareader.Forms
{
	public class FormRead : UserControl
	{
		private readonly PictureBox pictureBox;
		private readonly Button btnPrev;
		private readonly Button btnNext;
		private readonly Button btnBack;
		private FileInfo[]? images;
		private List<(string name, byte[] imageData)> imagesMem;
		private int currentIndex;
		public event Action? BackReq;
		private bool isMemory = false;

		//private float zoomFactor = 1.0f;
		//private const float zoomStep = 0.1f;
		//private const float maxZoom = 3.0f;
		//private const float minZoom = 0.5f;

		// Functions /////////////////////
		public void LoadImages(FileInfo[] imageFiles)
		{
			//zoomFactor = 1.5f;
			images = imageFiles;
			currentIndex = 0;
			ShowPage(0);
		}
		public void LoadImages(List<(string name, byte[] imageData)> imageFiles)
		{
			//zoomFactor = 1.5f;
			imagesMem = imageFiles;
			currentIndex = 0;
			isMemory = true;
			ShowPage(0);
		}
		private void ShowPage(int index)
		{
			if (isMemory)
			{
				if (imagesMem == null || index < 0 || index >= imagesMem.Count)
					return;

				currentIndex = index;

				// Dispose old image to free memory
				if (pictureBox.Image != null)
				{
					pictureBox.Image.Dispose();
					pictureBox.Image = null;
				}

				var imageData = imagesMem[index].imageData;
				using var ms = new MemoryStream(imageData);
				pictureBox.Image = Image.FromStream(ms);
			}
			else
			{
				if (images == null || index < 0 || index >= images.Length)
					return;

				currentIndex = index;

				// Dispose old image to free memory
				if (pictureBox.Image != null)
				{
					pictureBox.Image.Dispose();
					pictureBox.Image = null;
				}

				pictureBox.Image = Image.FromFile(images[currentIndex].FullName);
				// Text = $"Page {currentIndex + 1} / {images.Length} - {images[currentIndex].Name}";
			}
		}
		//private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
		//{
		//    if (e.Delta > 0)
		//    {
		//        zoomFactor += 0.1f;
		//    }
		//    else if (e.Delta < 0 && zoomFactor > 0.2f)
		//    {
		//        zoomFactor -= 0.1f;
		//    }

		//    // Apply zoom by resizing the PictureBox
		//    pictureBox.Width = (int)(pictureBox.Image.Width * zoomFactor);
		//    pictureBox.Height = (int)(pictureBox.Image.Height * zoomFactor);
		//}

		// Event Handlers /////////////////////
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
					btnPrev.PerformClick();
					return true;
				case Keys.Right:
					btnNext.PerformClick();
					return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		// Views /////////////////////
		public FormRead()
		{
			Dock = DockStyle.Fill;
			Visible = false;
			
			this.KeyDown += (s, e) => { };
			this.TabStop = true;

			pictureBox = new PictureBox
			{
				Dock = DockStyle.Fill,
				SizeMode = PictureBoxSizeMode.Zoom,
				BackColor = Color.Black
			};

			//pictureBox.MouseWheel += PictureBox_MouseWheel;
			//pictureBox.MouseEnter += (s, e) => pictureBox.Focus();

			var buttonPanel = new Panel
			{
				Dock = DockStyle.Bottom,
				Height = 50
			};

			btnPrev = new Button
			{
				Text = "Prev",
				Dock = DockStyle.Left,
				Width = 100
			};
			btnPrev.Click += (s, e) => ShowPage(currentIndex - 1);

			btnNext = new Button
			{
				Text = "Next",
				Dock = DockStyle.Right,
				Width = 100
			};
			btnNext.Click += (s, e) => ShowPage(currentIndex + 1);

			btnBack = new Button
			{
				Text = "Back",
				Dock = DockStyle.Fill
			};
			btnBack.Click += (s, e) => BackReq?.Invoke();

			buttonPanel.Controls.Add(btnPrev);
			buttonPanel.Controls.Add(btnNext);
			buttonPanel.Controls.Add(btnBack);

			Controls.Add(pictureBox);
			Controls.Add(buttonPanel);
		}
	}
}