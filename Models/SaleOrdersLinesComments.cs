using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goods.Models
{
    public class SaleOrdersLinesComments
    {
        [Key]
        public int CommentLineId { get; set; }
        [ForeignKey("Id")]
        public int DocId { get; set; }
        [ForeignKey("LineId")]
        public int LineId { get; set; }
        public string Comment { get; set; }
    }
}