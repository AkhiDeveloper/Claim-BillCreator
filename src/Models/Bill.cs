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

        public decimal DiscountAmount(decimal discountrate = 0.000m)
        {
            return decimal.Round(SubTotal() * discountrate, 2, MidpointRounding.ToPositiveInfinity);
        }

        public decimal TaxableAmount(decimal discountRate = 0.000m)
        {
            return decimal.Round(SubTotal() - DiscountAmount(discountRate), 2, MidpointRounding.ToPositiveInfinity);
        }

        public decimal TaxAmount(decimal taxRate = 0.13m, decimal discountRate = 0.000m)
        {
            return decimal.Round(TaxableAmount(discountRate) * taxRate, 2, MidpointRounding.ToPositiveInfinity);
        }

        public decimal TotalAmount(decimal taxRate = 0.13m, decimal discountRate = 0.000m)
        {
            return decimal.Round(TaxableAmount(discountRate) + TaxAmount(taxRate, discountRate), 2, MidpointRounding.ToPositiveInfinity);
        }
    }
}
