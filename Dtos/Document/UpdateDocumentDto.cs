using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goods.Dtos.Document
{
    public class UpdateDocumentDto
    {
        public int ID { get; set; }
        public char DocType { get; set; }
        public string BPCode { get; set; }
        public int CreatedBy { get; set; }
        public List<OrderLine> orderLines { get; set; }
        public string Comment { get; set; }
    }
}