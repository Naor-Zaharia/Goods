using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goods.Models
{
    public class OrderLineGetDocument
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public string ItemName { get; set; }
        public bool Active { get; set; }

    }
}