using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace Models.DataModels
{
    public class SEO : BaseDataModel
    {
        [StringLength(5)]
        public string Name { get; set; }

        [StringLength(5)]
        public string Ext { get; set; }

        [StringLength(5)]
        public string LocalExt { get; set; }

        public bool LocalMirrorFile { get; set; }
    }
}
