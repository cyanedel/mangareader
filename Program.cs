using mangareader.Forms;
using System.Text.Json;

namespace mangareader
{
  internal static class Program
  {
    [STAThread]
    static void Main()
    {
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }
}

class ConfigManager
{
  private static readonly string AppConfigFile = Path.Combine(AppContext.BaseDirectory, "appconfig.json");
  public static AppConfig LoadSettings()
  {
    // MessageBox.Show("Config:" + AppConfigFile);
    if (!File.Exists(AppConfigFile))
    {
      // Create default if missing
      var defaultSettings = new AppConfig { MangaRootDir = "C:\\YourDirectory" };
      SaveSettings(defaultSettings);
      return defaultSettings;
    }

    string json = File.ReadAllText(AppConfigFile);
    var settings = JsonSerializer.Deserialize<AppConfig>(json);

    return settings ?? new AppConfig { MangaRootDir = "" };
  }

  public static void SaveSettings(AppConfig settings)
  {
    string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(AppConfigFile, json);
  }
}
