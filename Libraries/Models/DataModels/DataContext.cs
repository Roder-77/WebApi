using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Models.DataModels
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Member> Member { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}
