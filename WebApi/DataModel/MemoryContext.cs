using Microsoft.EntityFrameworkCore;

namespace WebApi.DataModel
{
    public class MemoryContext : DbContext
    {
        public MemoryContext(DbContextOptions<MemoryContext> options) : base(options)
        { }

        public DbSet<Member> Member { get; set; } = null!;
    }
}
