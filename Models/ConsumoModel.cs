using MongoDB.Bson;

namespace GlobalSolution.Models
{
    public class ConsumoModel
    {
        public int Id { get; set; }
        public string Device { get; set; }
        public double PowerUsage { get; set; } // em kWh
        public DateTime Timestamp { get; set; }
    }
}
