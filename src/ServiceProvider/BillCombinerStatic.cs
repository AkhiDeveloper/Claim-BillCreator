using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public static class BillCombinerStatic
    {
        private readonly static IBillCombiner _combiner;
        static BillCombinerStatic()
        {
            _combiner = new BillCombiner();
        }

        public static async Task<IEnumerable<Bill>> CombineBillByOutlet(IEnumerable<Bill> bills)
        {
            return await _combiner.CombineBillByOutlet(bills);
        }
    }
}
