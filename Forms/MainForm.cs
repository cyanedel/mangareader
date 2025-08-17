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
		private readonly ReadControl readControl;
		private readonly PreviewControl previewControl;

		// Functions /////////////////////

		// Views /////////////////////
		public MainForm()
		{
			Text = "Manga Reader";
			Size = new Size(1000, 700);
			appConfig = ConfigManager.LoadSettings();
			readControl = new ReadControl();
			previewControl = new PreviewControl() {
				Dock = DockStyle.Fill,
			};
			previewControl.BookReadEvent += (s, e) => {
				if (e.imageFileList is List<(string name, byte[] imageData)>) {
					this.imageFilesMem = e.imageFileList;
					readControl.LoadImages(this.imageFilesMem);
					ToggleView("read");
				} else if (e.imageFIArray is FileInfo[]) {
					this.imageFiles = e.imageFIArray;
					readControl.LoadImages(this.imageFiles);
					ToggleView("read");
				}
			};

			readControl.BackReq += () => ToggleView("");
			Controls.Add(readControl);
			Controls.Add(previewControl);

			ToggleView("");
		}
		public void ToggleView(string state)
		{
			switch (state)
			{
				case "read":
					previewControl.Visible = false;
					readControl.Visible = true;
					readControl.Focus();
					Text = "Manga Reader - Read";
					break;
				default:
					previewControl.Visible = true;
					readControl.Visible = false;
					Text = "Manga Reader - List";
					break;
			}
		}
	}
}