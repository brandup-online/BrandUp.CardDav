using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IDavProperty : IXmlSerializable
    {
        string Name { get; }
        string Namespace { get; }
    }

    public class PropertyComparer : IEqualityComparer<IDavProperty>
    {
        public bool Equals(IDavProperty x, IDavProperty y) => x.Name == y.Name && x.Namespace == y.Namespace;


        public int GetHashCode([DisallowNull] IDavProperty obj) => $"{obj.Namespace}:{obj.Name}".GetHashCode();

    }
}