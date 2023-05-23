using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public interface IClaimCreator
    {
        public Claim CreateClaimFromBills(string companyName, IEnumerable<Bill> bills, decimal discount = 0.00m, decimal taxRate = 0.13m, int? year = null, int? month = null, string address = "", string phoneNumber = "");
    }
}
