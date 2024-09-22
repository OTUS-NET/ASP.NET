using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class ErrorMessage
    {
        public List<string> Errors { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }

        public ErrorMessage(string error, string path)
        {
            Errors = new List<string>() { error };
            Title = "Validation Error occured:";
            Path = path;
        }
    }
}
