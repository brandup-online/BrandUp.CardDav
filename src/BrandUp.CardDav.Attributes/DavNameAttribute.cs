namespace BrandUp.CardDav.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DavNameAttribute : Attribute
    {
        public DavNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Namespace = "DAV:";
        }

        public DavNameAttribute(string name, string @namespace)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }

        public string Name { get; init; }
        public string Namespace { get; init; }
    }
}
