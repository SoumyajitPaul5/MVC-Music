namespace MVC_Music.ViewModels
{
    public class CheckOptionVM
    {
        // Identifier for the option
        public int ID { get; set; }

        // Text to display for the option
        public string DisplayText { get; set; }

        // Indicates whether the option is assigned or checked
        public bool Assigned { get; set; }
    }
}
