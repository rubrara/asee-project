using CsvHelper;
using System.Globalization;

namespace PFMdotnet.Helpers.ParseCSV
{
    public class CsvParse<T>
    {

        public static List<T> ToList(IFormFile file)
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
    }
}

 