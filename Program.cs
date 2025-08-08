using mangareader.Forms;

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
      Application.Run(new FormMain());
    }
  }
}
