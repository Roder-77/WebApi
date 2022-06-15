#nullable disable


using System.ComponentModel;
using System.Runtime.Serialization;

namespace Models.Response
{
    public class Response<T>
    {
        [DataMember(Order = 1)]
        [DefaultValue(1)]
        public virtual int Code { get; set; }

        [DataMember(Order = 2)]
        public virtual string Message { get; set; }

        [DataMember(Order = 3)]
        public virtual T Data { get; set; }
    }
}
