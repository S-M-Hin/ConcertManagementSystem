
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.Entities
{
    public class Concert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The title cannot exceed 250 characters.")]
        public string Title { get; set; }


        [StringLength(250, ErrorMessage = "Performer name cannot exceed 250 characters.")]
        public string? Performer { get; set; }

       
        public int? VenueId { get; set; }
        public virtual Venue? Venue { get; set; }

        [Required]
        [Display(Name = "Concert Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Ticket Price")]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000.00.")]
        public decimal TicketPrice { get; set; }


        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available tickets must be a non-negative value.")]
        public int AvailableTickets { get; set; }

        [Display(Name = "Is Sold Out")]
        public bool IsSoldOut { get; set; }



        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
