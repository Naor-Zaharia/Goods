using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goods.Models
{
    public class PurchaseOrdersLines
    {
        [Key]
        public int LineId { get; set; }
        [ForeignKey("Id")]
        public int DocId { get; set; }
        [ForeignKey("LineId")]
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        [ForeignKey("Id")]
        public int CreatedBy { get; set; }
        [ForeignKey("Id")]
        public int? LastUpdatedBy { get; set; }

    }
}