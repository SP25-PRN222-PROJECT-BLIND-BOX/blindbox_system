using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using System.Data.Entity;

namespace BlindBoxShop.Repository
{
    public class BlindBoxImageRepository : RepositoryBase<BlindBoxImage>, IBlindBoxImageRepository
    {

        private readonly RepositoryContext _context;

        public BlindBoxImageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _context = repositoryContext;
        }

        public async Task<List<BlindBoxImage>> GetImageByBlindBoxId(Guid blindBoxId)
        {
            

            return await _context.BlindBoxImages.Where(image => image.BlindBoxId.Equals(blindBoxId)).ToListAsync();
        }
    }
}
