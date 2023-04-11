using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public interface IWholeSaleClaimCreator
    {
        WholeSaleClaim CreateClaimFromBills(IEnumerable<Bill> bills);
    }
}
