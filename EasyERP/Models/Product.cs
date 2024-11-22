using System.ComponentModel.DataAnnotations;
namespace EasyERP.Models
{
    public class Product
    {
        [Key]
        public int IntProductId { get; set; }
        public string StrProductName { get; set; }
        public decimal NumUnitPrice { get; set; }
        public decimal NumStock { get; set; }
    }
}
