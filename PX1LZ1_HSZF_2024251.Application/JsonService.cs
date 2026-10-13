using PX1LZ1_HSZF_2024251.Model;
using PX1LZ1_HSZF_2024251.Persistence.MsSql;
using Newtonsoft.Json;

namespace PX1LZ1_HSZF_2024251.Application
{
    public class JsonService
    {
        private readonly AppDbContext _dbContext;

        public JsonService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ProcessJsonFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"A megadott JSON fájl nem található: {filePath}");

            string jsonData = await File.ReadAllTextAsync(filePath);
            var root = JsonConvert.DeserializeObject<Root>(jsonData);

            foreach (var world in root.Worlds)
            {
                var existingWorld = _dbContext.Worlds.FirstOrDefault(w => w.Name == world.Name);
                if (existingWorld != null)
                {
                    foreach (var damage in world.Damages)
                    {
                        if (!existingWorld.Damages.Any(d => d.Description == damage.Description && d.Cost == damage.Cost))
                        {
                            existingWorld.Damages.Add(damage);
                        }
                    }
                }
                else
                {
                    _dbContext.Worlds.Add(world);
                }
            }

            await _dbContext.SaveChangesAsync();
            CreateDeadpoolFolders(root.Worlds);
        }

        private void CreateDeadpoolFolders(List<World> worlds)
        {
            string baseFolder = Path.Combine(Environment.CurrentDirectory, "Deadpool");
            Directory.CreateDirectory(baseFolder);

            foreach (var world in worlds)
            {
                string worldFolder = Path.Combine(baseFolder, world.Name);
                Directory.CreateDirectory(worldFolder);
            }
        }
    }
}
