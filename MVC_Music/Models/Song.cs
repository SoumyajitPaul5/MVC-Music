using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVC_Music.Models
{
    // Represents a song and includes validation logic
    public class Song : Auditable
    {
        // Primary key for the Song entity
        public int ID { get; set; }

        // Property to display a summary of the song
        [Display(Name = "Song")]
        public string Summary
        {
            get
            {
                return Title + " (" + Genre?.Name + ")";
            }
        }

        // Song title with validation
        [Required(ErrorMessage = "You cannot leave the Song title blank.")]
        [StringLength(80, ErrorMessage = "Song title cannot be more than 80 characters long.")]
        public string Title { get; set; }

        // Date the song was recorded with validation
        [Display(Name = "Date Recorded")]
        [Required(ErrorMessage = "You must enter the Date Recorded")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateRecorded { get; set; }

        // Details about the song
        public SongDetail SongDetail { get; set; } = new SongDetail();

        // Album ID and associated Album object with validation
        [Display(Name = "Album")]
        [Required(ErrorMessage = "You must select the Album.")]
        public int AlbumID { get; set; }
        public Album Album { get; set; }

        // Genre ID and associated Genre object with validation
        [Display(Name = "Genre")]
        [Required(ErrorMessage = "You must select the Genre for the song.")]
        public int GenreID { get; set; }
        public Genre Genre { get; set; }
    }
}
