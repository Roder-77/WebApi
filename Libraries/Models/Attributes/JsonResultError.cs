namespace Models.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class JsonResultError : Attribute
    {
    }
}
