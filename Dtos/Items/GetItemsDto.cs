using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goods.Dtos.Items
{
    public class GetItemsDto
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public bool Active { get; set; }

    }
}