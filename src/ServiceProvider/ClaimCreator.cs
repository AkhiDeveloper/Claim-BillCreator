using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public class ClaimCreator
        : IClaimCreator
    {
        public Claim CreateClaimFromBills(IEnumerable<Bill> bills, decimal discount = 0.00m, decimal taxRate = 0.13m)
        {
            var wholesaleclaim = new Claim();
            foreach (var bill in bills)
            {
                var claim_item = new WholesaleClaimItem();
                claim_item.Party = bill.OutletName;
                claim_item.Amount = bill.TotalAmount(taxRate, discount);
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
