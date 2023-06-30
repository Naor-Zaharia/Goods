using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goods.Models
{
    public class BusinessPartners
    {
        [Key]
        public string BPCode { get; set; }
        public string BPName { get; set; }
        [ForeignKey("BPType")]
        public char BPType { get; set; }
        public bool Active { get; set; }
    }
}