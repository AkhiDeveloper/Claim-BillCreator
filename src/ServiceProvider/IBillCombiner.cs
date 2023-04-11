using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public interface IBillCombiner
    {
        Task<IEnumerable<Bill>> CombineBillByOutlet(IEnumerable<Bill> bills);
    }
}
