using System.ComponentModel.DataAnnotations;

namespace Pkt.Models.Entites
{
    public class PktFor
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string PktName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set;} = string.Empty;

    }
}
