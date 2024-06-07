using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Customer : BaseEntity
    {
        public Customer()
        {
            Receipts = new List<Receipt>();
        }

        public int PersonId { get; set; }
        public Person Person { get; set; }
        public int DiscountValue { get; set; }
        public ICollection<Receipt> Receipts { get; set; }
    }

}
