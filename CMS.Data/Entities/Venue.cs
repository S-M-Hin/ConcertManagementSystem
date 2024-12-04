
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.Entities
{
    public class Venue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Venue name cannot exceed 250 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Location cannot exceed 500 characters.")]
        public string Location { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000.")]
        public int Capacity { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Contact phone cannot exceed 50 characters.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string ContactPhone { get; set; }

        [StringLength(250, ErrorMessage = "Email cannot exceed 250 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }


        [Display(Name = "Is Indoor")]
        public bool IsIndoor { get; set; }

        public virtual ICollection<Concert> Concerts { get; set; } = new List<Concert>();
    }
}
