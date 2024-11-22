using System.ComponentModel.DataAnnotations;

namespace EasyERP.Models
{
    public class Order
    {
        [Key]
        public int IntOrderId { get; set; }
        public int IntProductId { get; set; }
        public string StrCustomerName { get; set; }
        public decimal NumQuantity { get; set; }
        public DateTime DtOrderDate { get; set; }

        public Product Product { get; set; }

    }
}
