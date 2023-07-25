namespace PFMdotnet.Helpers
{
    public class StringToTypeParser
    {
        public static bool CanParseEnum<T>(string item) where T : struct
        {
            return Enum.TryParse<T>(item, out _);
        }
    }
}
