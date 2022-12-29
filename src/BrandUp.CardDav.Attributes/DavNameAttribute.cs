namespace BrandUp.CardDav.Attributes
{
    /// <summary>
    /// Binding a context property with xml property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DavNameAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Local name of CardDav xml property</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DavNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Namespace = "DAV:";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Local name of CardDav xml property</param>
        /// <param name="namespace">Namespase of property</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DavNameAttribute(string name, string @namespace)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }
        /// <summary>
        /// Local name of CardDav xml property
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Namespase of property
        /// </summary>
        public string Namespace { get; init; }
    }
}
