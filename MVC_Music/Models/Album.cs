using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Music.Models
{
    // Represents an album and includes validation logic
    public class Album : Auditable, IValidatableObject
    {
        public int ID { get; set; }

        // Property to display the full summary of the album
        [Display(Name = "Album")]
        public string FullSummary
        {
            get
            {
                return Name + " - " + YearProduced + " (" + Genre?.Name + ")";
            }
        }

        // Property to display a partial summary of the album
        [Display(Name = "Album")]
        public string PartSummary
        {
            get
            {
                return Name + " (Genre: " + Genre?.Name + ")";
            }
        }

        // Album name with validation
        [Required(ErrorMessage = "You cannot leave the Album name blank.")]
        [StringLength(50, ErrorMessage = "Album name cannot be more than 50 characters long.")]
        public string Name { get; set; }

        // Year the album was produced with validation
        [Display(Name = "Year Produced")]
        [Required(ErrorMessage = "You cannot leave the Year Produced blank.")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "The Year Produced must be entered as exactly 4 numeric digits.")]
        [StringLength(4)]
        public string YearProduced { get; set; }

        // Album price with validation
        [Required(ErrorMessage = "You must enter a price for the Album.")]
        [Range(1.0, 200000.00, ErrorMessage = "Price must be between $1.00 and $200,000.00")]
        [DataType(DataType.Currency)]
        public double Price { get; set; } = 19.99d;

        // Genre ID and associated Genre object with validation
        [Display(Name = "Genre")]
        [Required(ErrorMessage = "You must select the primary Genre of the Album.")]
        public int GenreID { get; set; }
        public Genre Genre { get; set; }

        // Collection of songs in the album
        public ICollection<Song> Songs { get; set; } = new HashSet<Song>();

        // Custom validation logic for the album
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Ensure the Year Produced is not more than one year in the future
            if ((int.Parse(YearProduced) - 1) > DateTime.Today.Year)
            {
                yield return new ValidationResult("Year Produced cannot be more than one year in the future.", new[] { "YearProduced" });
            }
        }
    }
}
