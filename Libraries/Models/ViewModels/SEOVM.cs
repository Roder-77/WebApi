#nullable disable warnings

namespace Models.ViewModels
{
    public class SEOVM
    {
        public string Name { get; set; }

        public string Ext { get; set; }

        public string LocalExt { get; set; }

        public bool LocalMirrorFile { get; set; }

        public string ImagePath
        {
            get
            {
                var basePath = $"/File/image/{Name}";

                if (LocalMirrorFile)
                {
                    return basePath + (string.IsNullOrWhiteSpace(Ext) ? Ext : $"-ogimge{Ext}");
                }
                else
                {
                    return basePath + (string.IsNullOrWhiteSpace(LocalExt) ? Ext : $"-ogimge{LocalExt}");
                }
            }
        }
    }
}
