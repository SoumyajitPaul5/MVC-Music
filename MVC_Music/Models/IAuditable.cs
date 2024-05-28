namespace MVC_Music.Models
{
    // Interface to define auditable properties
    internal interface IAuditable
    {
        // Property to store the username of the creator
        string CreatedBy { get; set; }

        // Property to store the creation timestamp
        DateTime? CreatedOn { get; set; }

        // Property to store the username of the last updater
        string UpdatedBy { get; set; }

        // Property to store the last update timestamp
        DateTime? UpdatedOn { get; set; }
    }
}
