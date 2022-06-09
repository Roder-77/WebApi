using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Models.DataModels
{
    public class MemoryContext : DbContext
    {
        public MemoryContext(DbContextOptions<MemoryContext> options) : base(options)
        { }

        public DbSet<Member> Member { get; set; }
    }
}
