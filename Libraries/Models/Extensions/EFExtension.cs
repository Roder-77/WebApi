using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Models.DataModels;

namespace Models.Extensions
{
    public static class EFExtension
    {
        /// <summary>
        /// 註冊所有實體
        /// </summary>
        /// <param name="modelBuilder">modelBuilder</param>
        public static void RegisterAllEntities(this ModelBuilder modelBuilder)
        {
            var baseDataModelType = typeof(BaseDataModel);
            var types = baseDataModelType.Assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && t != baseDataModelType && baseDataModelType.IsAssignableFrom(t));

            foreach (var type in types)
                modelBuilder.Entity(type);
        }

        /// <summary>
        /// 設定 Json 轉換
        /// </summary>
        /// <typeparam name="TProperty">實體屬性</typeparam>
        /// <param name="propertyBuilder">propertyBuilder</param>
        public static void SetJsonConversion<TProperty>(this PropertyBuilder<IEnumerable<TProperty>> propertyBuilder)
        {
            var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

            propertyBuilder.HasConversion(
                x => JsonSerializer.Serialize(x, jsonOptions),
                x => JsonSerializer.Deserialize<IEnumerable<TProperty>>(x, jsonOptions)!,
                new ValueComparer<IEnumerable<TProperty>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
                    c => c.ToList())
            );
        }
    }
}
