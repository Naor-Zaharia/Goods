using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goods.Models
{
    public class OrderLine
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }

    }
}