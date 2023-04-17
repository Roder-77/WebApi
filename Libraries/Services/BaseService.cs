using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.DataModels;
using Services.Repositories;

namespace Services
{
    public class BaseService<TEntity>
        where TEntity : BaseDataModel
    {
        protected readonly HttpContext _httpContext;

        protected readonly ILogger<BaseService<TEntity>> _logger;
        protected readonly IGenericRepository<TEntity> _repository;

        protected readonly IMapper _mapper;

        public BaseService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext!;
            _logger = _httpContext.RequestServices.GetRequiredService<ILogger<BaseService<TEntity>>>();
            _repository = _httpContext.RequestServices.GetRequiredService<IGenericRepository<TEntity>>();
            _mapper = _httpContext.RequestServices.GetRequiredService<IMapper>();
        }
    }
}
