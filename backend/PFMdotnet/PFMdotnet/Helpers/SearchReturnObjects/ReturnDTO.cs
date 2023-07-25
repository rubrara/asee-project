﻿namespace PFMdotnet.Models
{
    public class ReturnDTO<T>
    {

        public string Message { get; set; }
        public List<string>? Errors { get; set; }
        public T? Value { get; set; }

    }
}