using CsvHelper;
using System.Globalization;

namespace PFMdotnet.Helpers.ParseCSV
{
    public class CsvManip
    {

        public static List<T> ToList<T>(IFormFile file)
        {

            if (file == null || file.Length == 0) throw new ArgumentNullException();

            try
            {
                var streamReader = new StreamReader(file.OpenReadStream());

                var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

                return csvReader.GetRecords<T>().ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool IsValidCsvFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".csv" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}

 