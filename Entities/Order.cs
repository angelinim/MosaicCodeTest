namespace Mosaic2.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public float durationHours { get; set; }
        public string orderName { get; set; }
        public int bakerId { get; set; }
    }
}
