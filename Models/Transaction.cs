using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expence_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [Range(1,int.MaxValue,ErrorMessage ="Please Select category")]
        public int CategoryID { get; set; }
        public Category? Category { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "Amount Should be Greater than Zero")]
		public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime Date {  get; set; }= DateTime.Now;

        [NotMapped]
        public string TitleWithIcone
        {
			get
			{
                return Category != null? Category.Icone + " " + Category.Title: "";
            }
        }
        [NotMapped]
        public string? FormattedAmount
        { 
            get
            {
                return ((Category == null || Category.Type == "Expense") ? "- " : "+ ") + Amount.ToString("c0");

			} 
        }

	}
}
