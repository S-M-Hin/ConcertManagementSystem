
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.Entities
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Seat number cannot exceed 100 characters.")]
        public string SeatNumber { get; set; }

        [Required]
        [Display(Name = "Is Sold")]
        public bool IsSold { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between 0.01 and 1,000.00.")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }

        [StringLength(250, ErrorMessage = "Buyer name cannot exceed 250 characters.")]
        public string? BuyerName { get; set; }

        [StringLength(250, ErrorMessage = "Buyer email cannot exceed 250 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? BuyerEmail { get; set; }

        [Required]
        public int? ConcertId { get; set; }
        public virtual Concert? Concert { get; set; }
    }
}
