using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MVC_Music.ViewModels
{
    public class UserVM
    {
        // Identifier for the user
        public string Id { get; set; }

        // Username of the user
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        // List of roles assigned to the user
        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }
    }
}
