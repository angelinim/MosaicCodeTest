using System.ComponentModel.DataAnnotations;

namespace Mosaic2.Models
{
    public class OrderModel
    {
        [Required]
        public float durationHours { get; set; }
        [Required]
        public string orderName { get; set; }
    }
}
