using System.Text.RegularExpressions; 

namespace csreader
{
  public class FormMain : Form
  {
    private FlowLayoutPanel flowPanel;
    private Button btnRead;
    private FileInfo[] imageFiles = Array.Empty<FileInfo>();
    public FormMain()
    {
      Text = "Manga Reader";
      Size = new Size(1000, 700);

      var mainLayout = new TableLayoutPanel
      {
          Dock = DockStyle.Fill,
          RowCount = 2,
          ColumnCount = 1
      };
      mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
      mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
      Controls.Add(mainLayout);

      btnRead = new Button
      {
        Text = "Read",
        Dock = DockStyle.Fill,
        Height = 40
      };
      btnRead.Click += BtnRead_click;
      mainLayout.Controls.Add(btnRead, 0, 0);

      flowPanel = new FlowLayoutPanel()
      {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        WrapContents = true,
        FlowDirection = FlowDirection.LeftToRight,
        Padding = new Padding(10)
      };
      mainLayout.Controls.Add(flowPanel, 0, 1);

      LoadImages(@"C:\YourDirectory");
    }
    private void LoadImages(string fileDirectory)
    {
      DirectoryInfo d = new DirectoryInfo(fileDirectory);
      imageFiles = d.GetFiles("*.jpg")
        .OrderBy(f => ExtractNumber(f.Name))
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
    private int ExtractNumber(string fileName)
    {
      // Extract the first number from the filename
      var match = Regex.Match(fileName, @"\d+");
      return match.Success ? int.Parse(match.Value) : 0;
    }
    private void BtnRead_click(object sender, EventArgs e)
    {
      if (imageFiles == null || imageFiles.Length == 0)
      {
          MessageBox.Show("No images found to read.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
      }

      var readerForm = new FormRead(imageFiles);
      readerForm.ShowDialog();
    }
  }
}