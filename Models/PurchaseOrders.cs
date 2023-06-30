using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goods.Models
{
    public class PurchaseOrders
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("BPType")]
        public string BPCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        [ForeignKey("Id")]
        public int CreatedBy { get; set; }
        [ForeignKey("Id")]
        public int? LastUpdatedBy { get; set; }
    }
}