#nullable disable warnings

namespace Models.JobModels
{
    public sealed class JobModel : IEquatable<JobModel>
    {
        /// <summary>
        /// Job 識別碼
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 服務名稱
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 方法名稱
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Cron 表達式
        /// 5 fields: minute, hour, day of month, month, day of week
        /// 6 fields: second, minute, hour, day of month, month, day of week
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 時區
        /// https://en.wikipedia.org/wiki/List_of_tz_database_time_zones
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// 方法對應參數，照傳入順序
        /// </summary>
        public object[] Arguments { get; set; } = [];

        public bool Equals(JobModel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id &&
                   DisplayName == other.DisplayName &&
                   ServiceName == other.ServiceName &&
                   MethodName == other.MethodName &&
                   CronExpression == other.CronExpression &&
                   IsEnabled == other.IsEnabled &&
                   TimeZone == other.TimeZone &&
                   Arguments.SequenceEqual(other.Arguments);
        }
    }
}
