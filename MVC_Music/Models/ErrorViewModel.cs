namespace MVC_Music.Models
{
    // ViewModel to represent error information
    public class ErrorViewModel
    {
        // Property to store the unique request ID
        public string RequestId { get; set; }

        // Property to determine if the RequestId should be shown
        // Returns true if RequestId is not null or empty
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
