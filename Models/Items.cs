using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Goods.Models
{
    public class Items
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public bool Active { get; set; }
    }
}