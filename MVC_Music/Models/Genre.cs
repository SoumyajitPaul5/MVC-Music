using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Music.Models
{
    public class Genre
    {
        // Primary key for the Genre entity
        public int ID { get; set; }

        // Name of the genre
        [Display(Name = "Genre")]
        [Required(ErrorMessage = "You cannot leave the Genre blank.")]
        [StringLength(50, ErrorMessage = "Genre name cannot be more than 50 characters long.")]
        public string Name { get; set; }

        // Collection of related Album entities
        public ICollection<Album> Albums { get; set; } = new HashSet<Album>();

        // Collection of related Song entities
        public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
    }
}
