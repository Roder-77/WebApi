namespace Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IgnoreValidPermission : Attribute
    { }
}
