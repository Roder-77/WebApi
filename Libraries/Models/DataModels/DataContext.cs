using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json.Serialization;
using System.Text.Json;

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

            modelBuilder.Entity<Member>(action => { });

            RegisterAllEntities(modelBuilder);
        }


        /// <summary>
        /// 註冊所有實體
        /// </summary>
        /// <param name="modelBuilder">modelBuilder</param>
        private void RegisterAllEntities(ModelBuilder modelBuilder)
        {
            var baseDataModelType = typeof(BaseDataModel);
            var types = baseDataModelType.Assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && typeof(BaseDataModel).IsAssignableFrom(t) && t != baseDataModelType);

            foreach (var type in types)
                modelBuilder.Entity(type);
        }

        /// <summary>
        /// 設定 Json 轉換
        /// </summary>
        /// <typeparam name="TProperty">實體屬性</typeparam>
        /// <param name="propertyBuilder">propertyBuilder</param>
        private void SetJsonConversion<TProperty>(PropertyBuilder<TProperty> propertyBuilder)
        {
            var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

            propertyBuilder.HasConversion(
                x => JsonSerializer.Serialize(x, jsonOptions),
                x => JsonSerializer.Deserialize<TProperty>(x, jsonOptions)
            );
        }
    }
}
