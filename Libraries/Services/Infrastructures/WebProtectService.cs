#nullable disable warnings

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Models.Infrastructures;
using Services.Infrastructures;
using System.Text;

namespace PH_GOIF.Services.WebProtect
{
    public sealed class WebProtectService : BaseService
    {
        private const string PurposeName = "Demo";
        private readonly IDataProtector _protector;

        public WebProtectService(IDataProtectionProvider dataProtectionProvider, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _protector = dataProtectionProvider.CreateProtector(PurposeName);
        }

        #region Utilities

        /// <summary>
        /// 將字串編碼為 URL-safe Base64
        /// </summary>
        private static string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        /// <summary>
        /// 將 URL-safe Base64 解碼為字串
        /// </summary>
        private static string Base64UrlDecode(string input)
        {
            var base64 = input
                .Replace('-', '+')
                .Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        #endregion

        public string CreateToken(string orderId, int amount, int expiryMinutes = 5)
        {
            var model = WebProtectModel.Create(orderId, amount, DateTime.Now, expiryMinutes);
            return Encrypt(model.Plaintext);
        }

        public WebProtectModel? DecryptToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                var encrypted = Base64UrlDecode(token);
                var plaintext = _protector.Unprotect(encrypted);
                return WebProtectModel.Deserialize(plaintext);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt token");
                return null;
            }
        }

        public WebProtectModel? RenewToken(string token, string channelId, string orderId, int amount, string plate, int expiryMinutes = 5)
        {
            var model = DecryptToken(token);
            if (model is null)
                return null;

            if (!model.IsValid(channelId, orderId, amount, plate))
                return null;

            if (DateTime.Now > model.ExpireTime)
                return null;

            return model.Renew(DateTime.Now, expiryMinutes);
        }

        public bool ValidateToken(string token, string channelId, string orderId, int amount, string plate)
        {
            var model = DecryptToken(token);
            if (model is null)
                return false;

            if (!model.IsValid(channelId, orderId, amount, plate))
                return false;

            if (model.IsExpired())
                return false;

            return true;
        }

        public string Encrypt(string plaintext)
        {
            var encrypted = _protector.Protect(plaintext);
            return Base64UrlEncode(encrypted);
        }
    }
}
