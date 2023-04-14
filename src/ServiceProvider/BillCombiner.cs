using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public class BillCombiner
        : IBillCombiner
    {
        public async Task<IEnumerable<Bill>> CombineBillByOutlet(IEnumerable<Bill> bills)
        {
            IList<Bill> result = new List<Bill>();
            var outletgroup = bills.GroupBy(x => x.OutletId);
            foreach(var outlet in outletgroup)
            {
                Bill bill_new = new Bill();
                bill_new.OutletId = outlet.First().OutletId;
                bill_new.OutletName = outlet.First().OutletName;
                bill_new.Items = new List<BillItem>();
                foreach(var bill in outlet)
                {
                    bill_new.Items = bill_new.Items.Union(bill.Items).ToList();
                    bill_new.Items = (await this.CombineBillItems(bill_new.Items)).ToList();
                }
                result.Add(bill_new);
            }
            return result;
        }

        public async Task<IEnumerable<BillItem>> CombineBillItems(IEnumerable<BillItem> items)
        {
            var result = new List<BillItem>();
            var itemsgroup = items.GroupBy(x => x.Name);
            int sn = 0;
            foreach(var item in itemsgroup)
            {
                sn++;
                var new_item = new BillItem();
                new_item.SN = sn;
                new_item.Name = item.First().Name;
                new_item.Rate = item.First().Rate;
                new_item.Quantity = item.Sum(x => x.Quantity);
                new_item.DiscountRate = item.Max(x  => x.DiscountRate); 
                result.Add(new_item);
            }
            return result;
        }
    }
}
