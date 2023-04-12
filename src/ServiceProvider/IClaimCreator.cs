﻿using src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.ServiceProvider
{
    public interface IClaimCreator
    {
        Claim CreateClaimFromBills(IEnumerable<Bill> bills);
    }
}
