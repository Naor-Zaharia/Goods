using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goods.Dtos.Document
{
    public class GetDocumentDto
    {
        public int ID { get; set; }
        public char DocType { get; set; }
        public string BPCode { get; set; }
        public int CreatedBy { get; set; }
        public List<OrderLineGetDocument> orderLines { get; set; }
        public string Comment { get; set; }
        public string BPName { get; set; }
        public bool Active { get; set; }
        public string FullName { get; set; }

    }
}