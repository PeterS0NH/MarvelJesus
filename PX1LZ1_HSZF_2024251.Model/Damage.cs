namespace PX1LZ1_HSZF_2024251.Model
{
    public class Damage
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string Category { get; set; } = string.Empty;
        public string DamageType { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public List<string> InvolvedParties { get; set; } = new List<string>();
        public string InsuranceCoverage { get; set; } = string.Empty;
        public string RepairDuration { get; set; } = string.Empty;
        public int WorldId { get; set; }
        public World World { get; set; }
    }
}
