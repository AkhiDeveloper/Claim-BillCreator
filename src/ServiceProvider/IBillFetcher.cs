using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    internal interface IBillFetcher
    {
        Task<IEnumerable<Bill>> FetchInvoices(DateOnly from, DateOnly to, string? userId);
    }
}
