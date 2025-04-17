using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using Models.ViewModels;

namespace Services
{
    public class ProductService : BaseService<Product>
    {
        public ProductService(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public async Task<ProductVM> Get(int id)
        {
            var product = await _repository.Table
                .Include(x => x.Images.Take(3))
                .Include(x => x.NewsRelations).ThenInclude(x => x.News)
                .Include(x => x.Specs.Take(4))
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<ProductVM>(product);
        }
    }
}
