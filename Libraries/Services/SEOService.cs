using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using Models.ViewModels;

namespace Services
{
    public class SEOService : BaseService<SEO>
    {
        public SEOService(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public async Task<SEOVM?> Get(int id)
        {
            var metaImage = await _repository.Table.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<SEOVM>(metaImage);
        }
    }
}
