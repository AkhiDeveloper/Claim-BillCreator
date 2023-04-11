using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    internal class AmountHelper
        : IAmountHelper
    {
        public decimal CalculateDiscountRate(decimal undiscountedAmount, decimal discountedAmount)
        {
            return (1 - discountedAmount / undiscountedAmount);
        }
    }
}
