namespace MVC_Music.ViewModels
{
    // Interface for email configuration settings
    public interface IEmailConfiguration
    {
        // SMTP server address
        string SmtpServer { get; }

        // SMTP server port
        int SmtpPort { get; }

        // Name used in the "From" field of the email
        string SmtpFromName { get; set; }

        // SMTP username for authentication
        string SmtpUsername { get; set; }

        // SMTP password for authentication
        string SmtpPassword { get; set; }
    }
}
