namespace Common.Helpers
{
    public static class ConvertHelper
    {
        public static T ChangeType<T>(object data) => (T)Convert.ChangeType(data, typeof(T));
    }
}