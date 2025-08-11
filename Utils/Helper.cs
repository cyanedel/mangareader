using System.Text.RegularExpressions; 

namespace mangareader.Utils
{
  public static class Helper
  {
    public static int ExtractNumber(string fileName)
    {
      // Extract the first number from the filename
      var match = Regex.Match(fileName, @"\d+");
      return match.Success ? int.Parse(match.Value) : 0;
		}
		public static bool IsImageFile(string fileName)
		{
			string ext = Path.GetExtension(fileName).ToLower();
			return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
		}
		public static bool IsArchive(string filePath)
		{
			string ext = Path.GetExtension(filePath).ToLower();
			return ext == ".zip" || ext == ".rar";
		}
	}
}