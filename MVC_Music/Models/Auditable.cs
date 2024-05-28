using System.ComponentModel.DataAnnotations;

namespace MVC_Music.Models
{
    // Abstract class representing common audit properties
    public abstract class Auditable : IAuditable
    {
        // Property to store the username of the creator
        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        // Property to store the creation timestamp
        [ScaffoldColumn(false)]
        public DateTime? CreatedOn { get; set; }

        // Property to store the username of the last updater
        [ScaffoldColumn(false)]
        [StringLength(256)]
        public string UpdatedBy { get; set; }

        // Property to store the last update timestamp
        [ScaffoldColumn(false)]
        public DateTime? UpdatedOn { get; set; }

        // Property to store the row version for concurrency control
        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }
    }
}
