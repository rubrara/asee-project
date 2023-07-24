namespace PFMdotnet.Models
{
    public class ReturnDTO<T>
    {

        public string Process { get; set; }
        public List<string>? Errors { get; set; }
        public T? Value { get; set; }

    }
}
