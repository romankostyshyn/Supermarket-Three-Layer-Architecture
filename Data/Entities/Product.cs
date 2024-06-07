using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Product : BaseEntity
    {
        public Product()
        {
            ReceiptDetails = new List<ReceiptDetail>();
        }

        public int ProductCategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }

}
