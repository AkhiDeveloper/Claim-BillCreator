using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public interface IClaimFileCreator : IDisposable
    {
        Task CreateFile(IEnumerable<Bill> bills, string? filepath = null, decimal discount = 0.00m, decimal taxRate = 0.13m);

        Task CreateFile(Claim claim, IEnumerable<Bill> bills, string? filepath = null, decimal discount = 0.00m, decimal taxRate = 0.13m);
    }
}
