namespace Services.Extensions
{
    public static class HttpMethodExtension
    {
        public static bool HasBody(this HttpMethod httpMethod) => httpMethod != HttpMethod.Get
            && httpMethod != HttpMethod.Head && httpMethod != HttpMethod.Get
            && httpMethod != HttpMethod.Options && httpMethod != HttpMethod.Trace;
    }
}
