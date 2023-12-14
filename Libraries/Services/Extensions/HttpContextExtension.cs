using Microsoft.AspNetCore.Http;
using Models.DataModels;

namespace Services.Extensions
{
    public static class HttpContextExtension
    {
        /// <summary>
        /// 取得暫存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetItem<T>(this HttpContext httpContext, string key)
            => (T)httpContext.Items[key]!;

        /// <summary>
        /// 取得帳號資料
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static Member GetMember(this HttpContext httpContext)
            => httpContext.GetItem<Member>("Member");
    }
}
