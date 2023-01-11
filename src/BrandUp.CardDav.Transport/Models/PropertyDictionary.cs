using BrandUp.CardDav.Transport.Models.Abstract;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BrandUp.CardDav.Transport.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyDictionary : IReadOnlyDictionary<IDavProperty, string>
    {
        readonly IDictionary<IDavProperty, string> properties;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PropertyDictionary(IDictionary<IDavProperty, string> properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            this.properties = new Dictionary<IDavProperty, string>(properties, new PropertyComparer());
        }

        #region IReadOnlyDictionary members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[IDavProperty key] => properties[key];

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> Keys => properties.Keys;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Values => properties.Values;

        /// <summary>
        /// 
        /// </summary>
        public int Count => properties.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(IDavProperty key) => properties.ContainsKey(key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<IDavProperty, string>> GetEnumerator() => properties.GetEnumerator();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(IDavProperty key, [MaybeNullWhen(false)] out string value) => properties.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();

        #endregion
    }

    internal class PropertyComparer : IEqualityComparer<IDavProperty>
    {
        public bool Equals(IDavProperty x, IDavProperty y) => x.Name == y.Name && x.Namespace == y.Namespace;


        public int GetHashCode([DisallowNull] IDavProperty obj) => $"{obj.Namespace}:{obj.Name}".GetHashCode();
    }
}
