using BrandUp.CardDav.Transport.Abstract.Properties;
using System.Diagnostics.CodeAnalysis;

namespace BrandUp.CardDav.Transport.Helpers
{
    internal class PropertyComparer : IEqualityComparer<IDavProperty>
    {
        public bool Equals(IDavProperty x, IDavProperty y) => x.Name == y.Name && x.Namespace == y.Namespace;

        public int GetHashCode([DisallowNull] IDavProperty obj) => $"{obj.Namespace}:{obj.Name}".GetHashCode();
    }
}
