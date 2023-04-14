using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Models
{
    public class Bill
    {
        public string OutletId { get; set; }
        public string OutletName { get; set; }
        public IList<BillItem> Items { get; set; }

        public decimal SubTotal()
        {
            return decimal.Round(Items.Sum(x => x.Amount), 2, MidpointRounding.ToPositiveInfinity);
        }

        public decimal Discount(decimal discount = 0.00m)
        {
            return decimal.Round(this.SubTotal() * discount, 2, MidpointRounding.ToPositiveInfinity);
        }

        public decimal TaxableAmount(decimal discount = 0.00m)
        {
            return decimal.Round(SubTotal() - Discount(discount), 2, MidpointRounding.ToPositiveInfinity);
        }

        public decimal TaxAmount(decimal taxRate = 0.13m, decimal discount = 0.00m)
        {
            return decimal.Round(TaxableAmount(discount) * taxRate, 2, MidpointRounding.ToPositiveInfinity);
        }

        public decimal TotalAmount(decimal taxRate = 0.13m, decimal discount = 0.00m)
        {
            return decimal.Round(TaxableAmount(discount) + TaxAmount(taxRate, discount), 2, MidpointRounding.ToPositiveInfinity);
        }
    }
}
