namespace GoldHelpers.Models
{
    public class Rootobject
    {
        public Logging? Logging { get; set; }
        public Projecturls? ProjectURLs { get; set; }
    }

    public class Logging
    {
        public Loglevel? LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string? Default { get; set; }
        public string? MicrosoftAspNetCore { get; set; }
    }

    public class Projecturls
    {
        public string? Accounting { get; set; }
    }
}
