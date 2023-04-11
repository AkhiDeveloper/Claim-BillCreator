using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    internal static class AmountHelperStatic
    {
        private readonly static IAmountHelper _amountHelper;

        static AmountHelperStatic()
        {
            _amountHelper = new AmountHelper();
        }

        public static decimal CalculateDiscountRate(decimal undiscountedAmount, decimal discountedAmount)
        {
            return _amountHelper.CalculateDiscountRate(undiscountedAmount, discountedAmount);
        }
    }
}
