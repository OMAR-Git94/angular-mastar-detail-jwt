using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsAPI.Models
{
    [Table(nameof(Product))]
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string ProductNumber { get; set; } = default!;

        public string Color { get; set; } = default!;

        [Required]
        public decimal StandardCost { get; set; }

        [Required]
        public decimal ListPrice { get; set;}

        [Required]
        public int Size { get; set;}

        [Required]
        public decimal Weight { get; set;}

        [ForeignKey(nameof(ProductCategory))]
        public int ProductCategoryID { get; set; }

        public ProductCategory? ProductCategory { get; set; }
    }
}
