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
  }
}