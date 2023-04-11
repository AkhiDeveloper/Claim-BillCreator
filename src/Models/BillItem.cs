using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Models
{
    public class BillItem
    {
        public int SN { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal Amount { get
            {
                return (Quantity * Rate) * (1 - DiscountRate);
            }
        }
    }
}
