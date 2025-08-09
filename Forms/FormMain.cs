using mangareader.Utils;

namespace mangareader.Forms
{
  public class FormMain : Form
  {
    private readonly AppConfig appConfig;
    private FileInfo[] imageFiles = Array.Empty<FileInfo>();
    private readonly FlowLayoutPanel flowPanel;
    private readonly Panel panelThumbnails;
    private readonly FormRead formRead;
    private readonly Button btnRead;

    // Functions /////////////////////
    private void LoadImages(string fileDirectory)
    {
      DirectoryInfo d = new(fileDirectory);
      imageFiles = d.GetFiles("*.jpg")
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

    // Views /////////////////////
    public FormMain()
    {
      Text = "Manga Reader";
      Size = new Size(1000, 700);
      appConfig = ConfigManager.LoadSettings();

      panelThumbnails = new Panel();
      btnRead = new Button();
      flowPanel = new FlowLayoutPanel();
      formRead = new FormRead();

      InitPanelThumbnails();
      InitPanelRead();
      LoadImages(appConfig.MangaRootDir);

      ToggleView("");
    }
    private void ToggleView(string state)
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
      btnRead.Click += (s, e) =>
      {
        formRead.LoadImages(imageFiles);
        ToggleView("read");
      };

      flowPanel.Dock = DockStyle.Fill;
      flowPanel.AutoScroll = true;
      flowPanel.WrapContents = true;
      flowPanel.FlowDirection = FlowDirection.LeftToRight;
      flowPanel.Padding = new Padding(10);
        
      panelThumbnails.Controls.Add(flowPanel);
      panelThumbnails.Controls.Add(btnRead);
      Controls.Add(panelThumbnails);
    }

    private void InitPanelRead()
    {
      formRead.BackReq += () => ToggleView("");
      Controls.Add(formRead);
    }
  }
}