namespace PX1LZ1_HSZF_2024251.Model
{
    public class World
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentYear { get; set; }
        public ICollection<Damage> Damages { get; set; } = new List<Damage>();
    }
}
