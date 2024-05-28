using System.ComponentModel.DataAnnotations;

namespace MVC_Music.Models
{
    // Represents detailed information about a song
    public class SongDetail
    {
        // Primary key for the SongDetail entity
        public int ID { get; set; }

        // Comments about the song with validation
        [StringLength(100, ErrorMessage = "Only 100 characters for comments.")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; } = "";

        // Length of the song with validation
        [Required(ErrorMessage = "You must enter a length for the song.")]
        public TimeSpan Length { get; set; } = default;

        // Rating of the song
        public Rating Rating { get; set; }

        // Foreign key to the related Song entity
        public int SongID { get; set; }

        // Navigation property to the related Song entity
        public Song Song { get; set; }
    }
}
