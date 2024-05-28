using System.ComponentModel.DataAnnotations;

namespace MVC_Music.Models
{
    public enum Rating
    {
        Terrible,
        [Display(Name ="Not too bad.")]
        Notbad,
        Ok,
        Good,
        Great
    }
}
