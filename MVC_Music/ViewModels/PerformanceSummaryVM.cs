using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MVC_Music.ViewModels
{
    public class PerformanceSummaryVM
    {
        public int ID { get; set; }

        // Displays the full name of the musician
        [Display(Name = "Musician")]
        public string FullName
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                        (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        // First name of the musician
        public string FirstName { get; set; }

        // Middle name of the musician
        public string MiddleName { get; set; }

        // Last name of the musician
        public string LastName { get; set; }

        // Displays the average earnings as currency
        [Display(Name = "Average Earnings")]
        [DataType(DataType.Currency)]
        public double AverageFeePaid { get; set; }

        // Displays the highest fee paid as currency
        [Display(Name = "Highest Fee Paid")]
        [DataType(DataType.Currency)]
        public double HighestFeePaid { get; set; }

        // Displays the lowest fee paid as currency
        [Display(Name = "Lowest Fee Paid")]
        [DataType(DataType.Currency)]
        public double LowestFeePaid { get; set; }

        // Displays the number of performances
        [Display(Name = "Number of Performances")]
        public int NumberOfPerformances { get; set; }

        // Displays the number of songs
        [Display(Name = "Number of Songs")]
        public int NumberOfSongs { get; set; }
    }
}
