using System.ComponentModel.DataAnnotations;

namespace Goods.Models
{
    public class BPType
    {
        [Key]
        public char TypeCode { get; set; }
        public string TypeName { get; set; }

    }
}