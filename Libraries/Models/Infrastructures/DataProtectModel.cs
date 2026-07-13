#nullable disable warnings

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models.Infrastructures
{
    public record WebProtectModel
    {
        public string OrderId { get; init; }

        public int Amount { get; init; }

        public DateTime StartTime { get; init; }

        public DateTime ExpireTime { get; init; }

        public DateTime CurrentStepExpirTime { get; init; }

        public int RemainingSeconds => (int)Math.Max(0, (CurrentStepExpirTime - DateTime.Now).TotalSeconds);

        public string Nonce { get; init; } = Guid.NewGuid().ToString("N");

        [JsonIgnore]
        public string Plaintext => GeneratePlaintext();

        public static WebProtectModel Create(string orderId, int amount, DateTime? now = null, int expiryMinutes = 5)
        {
            now ??= DateTime.Now;
            return new()
            {
                OrderId = orderId,
                Amount = amount,
                StartTime = now.Value,
                ExpireTime = now.Value.AddMinutes(20),
                CurrentStepExpirTime = now.Value.AddMinutes(expiryMinutes)
            };
        }

        public static WebProtectModel? Deserialize(string plaintext)
        {
            return JsonSerializer.Deserialize<WebProtectModel>(plaintext);
        }

        public WebProtectModel Renew(DateTime? now = null, int expiryMinutes = 5)
        {
            now ??= DateTime.Now;
            return this with { CurrentStepExpirTime = now.Value.AddMinutes(expiryMinutes) };
        }

        public bool IsValid(string orderId, int amount)
        {
            return OrderId == orderId
                && Amount == amount;
        }

        public bool IsValid(int orderId, int amount)
        {
            return IsValid(orderId.ToString(), amount);
        }

        public bool IsExpired()
        {
            var now = DateTime.Now;
            return now > CurrentStepExpirTime || now > ExpireTime;
        }

        private string GeneratePlaintext()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
