using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DataModels
{
    public class BaseDataModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
    }

    public interface ICreateEntity
    {
        public long CreateTime { get; set; }
    }

    public interface ICreatorEntity : ICreateEntity
    {
        public int Creator { get; set; }
    }

    public interface IUpdateEntity
    {
        public long UpdateTime { get; set; }
    }

    public interface IUpdaterEntity : IUpdateEntity
    {
        public int Updater { get; set; }
    }

    public interface ISoftDeleteEntity
    {
        /// <summary>
        /// 是否刪除
        /// </summary>
        public bool IsDeleted { get; set; }
    }

    public interface IActiveEntity
    {
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; }
    }
}
