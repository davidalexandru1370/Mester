using System.ComponentModel.DataAnnotations.Schema;

namespace Registry.Models
{
    public class TradesManImages
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("TradesMan")]
        public Guid TradesmanId { get; set; }

        public TradesMan TradesMan { get; set; }
    }
}
