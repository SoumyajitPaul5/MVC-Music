
namespace MVC_Music.ViewModels
{
    // Configuration settings for email
    public class EmailConfiguration : IEmailConfiguration
    {
        // SMTP server address
        public string SmtpServer { get; set; }

        // SMTP server port
        public int SmtpPort { get; set; }

        // Name used in the "From" field of the email
        public string SmtpFromName { get; set; }

        // SMTP username for authentication
        public string SmtpUsername { get; set; }

        // SMTP password for authentication
        public string SmtpPassword { get; set; }
    }
}
