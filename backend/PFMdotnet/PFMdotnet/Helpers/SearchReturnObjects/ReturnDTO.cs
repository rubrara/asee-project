using System.Text.Json.Serialization;

namespace PFMdotnet.Models
{
    public class ReturnDTO<T>
    {

        public string Message { get; set; }
        public List<string>? Errors { get; set; } = null;
        public T? Result { get; set; } = default;

    }
}
