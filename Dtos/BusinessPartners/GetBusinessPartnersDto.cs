using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goods.Dtos.BusinessPartners
{
    public class GetBusinessPartnersDto
    {
        public string BPCode { get; set; }
        public string BPName { get; set; }
        public char BPType { get; set; }
        public bool Active { get; set; }

    }
}