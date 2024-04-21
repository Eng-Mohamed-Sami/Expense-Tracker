using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expence_Tracker.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName ="nvarchar(50)")]
        [Required(ErrorMessage ="Title Is Required")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string Icone { get; set; } = "";

        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Expense";  //Default is Expence.

        [NotMapped]
        public string? TitleWithIcone 
        { 
            get
            {
                return  this.Icone+ " " +this.Title;
            }
        }
    }
}
