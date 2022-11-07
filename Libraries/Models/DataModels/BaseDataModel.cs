using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DataModels
{
    public class BaseDataModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    public interface ICreateEntity
    {
        public long CreateTime { get; set; }

        public int Creator { get; set; }
    }

    public interface IUpdateEntity
    {
        public long UpdateTime { get; set; }

        public int Updater { get; set; }
    }

    public interface ISoftDelete
    {
        /// <summary>
        /// 是否刪除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
