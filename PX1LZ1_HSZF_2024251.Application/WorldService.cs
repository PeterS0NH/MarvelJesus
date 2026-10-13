using Microsoft.EntityFrameworkCore;
using PX1LZ1_HSZF_2024251.Model;
using PX1LZ1_HSZF_2024251.Persistence.MsSql;

namespace PX1LZ1_HSZF_2024251.Application
{
    public class WorldService
    {
        private readonly AppDbContext _dbContext;
        public event EventHandler<DamageEventArgs> HighValueDamageAdded;

        public WorldService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddWorldAsync(string name, int year)
        {
            if (_dbContext.Worlds.Any(w => w.Name == name))
                throw new Exception("Ilyen nevű világ már létezik.");

            var world = new World { Name = name, CurrentYear = year };
            _dbContext.Worlds.Add(world);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddDamageToWorldAsync(string worldName, string description, decimal cost, string category, string damageType, string impact, List<string> involvedParties, string insuranceCoverage, string repairDuration)
        {
            var world = _dbContext.Worlds.FirstOrDefault(w => w.Name == worldName);
            if (world == null)
                throw new Exception("A megadott világ nem található.");

            var damage = new Damage
            {
                Description = description,
                Cost = cost,
                Category = category,
                DamageType = damageType,
                Impact = impact,
                InvolvedParties = involvedParties,
                InsuranceCoverage = insuranceCoverage,
                RepairDuration = repairDuration,
                WorldId = world.Id
            };

            world.Damages.Add(damage);

            if (cost > 1000000)
            {
                OnHighValueDamageAdded(new DamageEventArgs(worldName, description, cost));
            }

            await _dbContext.SaveChangesAsync();
        }

        protected virtual void OnHighValueDamageAdded(DamageEventArgs e)
        {
            HighValueDamageAdded?.Invoke(this, e);
        }

        public async Task ListDamagesWithPaginationAsync(string worldName, int pageSize = 5)
        {
            var world = await _dbContext.Worlds
                .Include(w => w.Damages)
                .FirstOrDefaultAsync(w => w.Name == worldName);

            if (world == null)
                throw new Exception("A megadott világ nem található.");

            int totalDamages = world.Damages.Count;
            int totalPages = (int)Math.Ceiling((double)totalDamages / pageSize);
            int currentPage = 1;

            while (true)
            {
                var damages = world.Damages
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                Console.Clear();
                Console.WriteLine($"Világ: {world.Name}");
                Console.WriteLine($"Oldal {currentPage}/{totalPages}");
                Console.WriteLine("=======================");

                foreach (var damage in damages)
                {
                    Console.WriteLine($"Leírás: {damage.Description}");
                    Console.WriteLine($"Költség: ${damage.Cost:N0}");
                    Console.WriteLine($"Kategória: {damage.Category}");
                    Console.WriteLine($"Javítási időtartam: {damage.RepairDuration}");
                    Console.WriteLine("-----------------------");
                }

                Console.WriteLine("n: Következő oldal, p: Előző oldal, q: Kilépés");
                var input = Console.ReadLine();

                if (input == "n" && currentPage < totalPages)
                    currentPage++;
                else if (input == "p" && currentPage > 1)
                    currentPage--;
                else if (input == "q")
                    break;
                else
                    Console.WriteLine("Érvénytelen bemenet. Próbálja újra.");
            }
        }
    }
}
