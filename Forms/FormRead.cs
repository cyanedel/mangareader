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
    private int currentIndex;
    public event Action? BackReq;

    // Functions /////////////////////
    public void LoadImages(FileInfo[] imageFiles)
    {
      images = imageFiles;
      currentIndex = 0;
      ShowPage(0);
    }
    private void ShowPage(int index)
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

    // Event Handlers /////////////////////
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Left)
      {
        btnPrev.PerformClick();
      }
      else if (keyData == Keys.Right)
      {
        btnNext.PerformClick();
      }
      return true; // mark handled (prevent normal navigation)
    }

    // Views /////////////////////
    public FormRead()
    {
      Dock = DockStyle.Fill;
      Visible = false;

      Debug.WriteLine($"Form Read");
      this.KeyDown += (s, e) =>
      {
          Debug.WriteLine($"Key: {e.KeyCode}");
      };

      this.TabStop = true; // allow focus

      pictureBox = new PictureBox
      {
        Dock = DockStyle.Fill,
        SizeMode = PictureBoxSizeMode.Zoom,
        BackColor = Color.Black
      };

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