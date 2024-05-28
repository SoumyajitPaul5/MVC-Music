namespace MVC_Music.ViewModels
{
    public class RoleVM
    {
        // Identifier for the role
        public string RoleId { get; set; }

        // Name of the role
        public string RoleName { get; set; }

        // Indicates whether the role is assigned to a user
        public bool Assigned { get; set; }
    }
}
