using Microsoft.EntityFrameworkCore;
using Models.Extensions;

#nullable disable

namespace Models.DataModels
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // add more..
            modelBuilder.Entity<Member>(action => { });

            modelBuilder.RegisterAllEntities();
        }
    }
}
