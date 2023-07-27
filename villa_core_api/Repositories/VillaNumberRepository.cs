

using VillaApi.Entities;
using VillaApi.Interfaces;

namespace VillaApi.Repositories
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ModelAppContext _db;
        public VillaNumberRepository(ModelAppContext db) : base(db)
        {
            _db = db;
        }
    }
}