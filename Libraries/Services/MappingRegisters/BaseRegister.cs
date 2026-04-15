using Mapster;
using static Services.Extensions.PaginationExtension;

namespace Services.MappingRegisters
{
    public class BaseRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig(typeof(PaginationList<>), typeof(PaginationList<>));
        }
    }
}
