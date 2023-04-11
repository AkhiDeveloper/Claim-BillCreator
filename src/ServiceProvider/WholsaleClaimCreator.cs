using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public class WholsaleClaimCreator
        : IWholeSaleClaimCreator
    {
        public WholeSaleClaim CreateClaimFromBills(IEnumerable<Bill> bills)
        {
            var wholesaleclaim = new WholeSaleClaim();
            foreach (var bill in bills)
            {
                var claim_item = new WholesaleClaimItem();
                claim_item.Party = bill.OutletName;
                claim_item.Amount = bill.TotalAmount();
                claim_item.Discount = 0.01m;
                if(claim_item.Amount > 1_00_000)
                {
                    claim_item.Discount = 0.02m;
                }
                wholesaleclaim.AddPartyClaim(claim_item);
            }
            return wholesaleclaim;
        }
    }
}
