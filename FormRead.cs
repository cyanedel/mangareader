namespace csreader
{
  public class FormRead : Form
  {
    private PictureBox pictureBox;
    private Button prevButton;
    private Button nextButton;
    private FileInfo[] images;
    private int currentIndex;
    public FormRead(FileInfo[] imageFiles)
    {
      images = imageFiles;
      currentIndex = 0;

      Text = "Manga Reader - Page View";
      WindowState = FormWindowState.Maximized;

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

      prevButton = new Button
      {
        Text = "Prev",
        Dock = DockStyle.Left,
        Width = 100
      };
      prevButton.Click += (s, e) => ShowPage(currentIndex - 1);

      nextButton = new Button
      {
        Text = "Next",
        Dock = DockStyle.Right,
        Width = 100
      };
      nextButton.Click += (s, e) => ShowPage(currentIndex + 1);

      buttonPanel.Controls.Add(prevButton);
      buttonPanel.Controls.Add(nextButton);

      Controls.Add(pictureBox);
      Controls.Add(buttonPanel);

      ShowPage(0);
    }
    
    private void ShowPage(int index)
    {
      if (index < 0 || index >= images.Length)
        return;

      currentIndex = index;

      // Dispose old image to free memory
      if (pictureBox.Image != null)
      {
        pictureBox.Image.Dispose();
        pictureBox.Image = null;
      }

      pictureBox.Image = Image.FromFile(images[currentIndex].FullName);
      Text = $"Page {currentIndex + 1} / {images.Length} - {images[currentIndex].Name}";
    }
  }
}