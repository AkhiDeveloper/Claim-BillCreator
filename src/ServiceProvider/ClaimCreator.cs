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
        public Claim CreateClaimFromBills(string companyName, IEnumerable<Bill> bills, decimal discount = 0.00m, decimal taxRate = 0.13m, int? year = null, int? month = null, string address = "", string phoneNumber="")
        {
            year = year ?? DateTime.Now.Year;
            month = month ?? DateTime.Now.Month;
            var wholesaleclaim = new Claim(companyName, address: address, year: year.Value, month: month.Value);
            wholesaleclaim.AddPhoneNumber(phoneNumber);
            foreach (var bill in bills)
            {
                var claim_item = new CliamItem();
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
