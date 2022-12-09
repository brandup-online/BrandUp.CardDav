using BrandUp.CardDav.Transport.Models.Abstract;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class PropfindResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public string StatusCode { get; init; }

        IEnumerable<IResponseResource> IResponse.Resources => Resources;

        public PropfindResponseResource[] Resources { get; init; }
        public static PropfindResponse Create(HttpResponseMessage message)
        {
            var resourses = XmlSerializer.DeserializeToResourses(message.Content.ReadAsStream());
            var newResources = new List<PropfindResponseResource>();
            foreach (var resource in resourses)
            {
                newResources.Add(new() { Endpoint = resource.Endpoint, FoundProperties = resource.FoundProperties });
            }
            return new() { IsSuccess = message.IsSuccessStatusCode, StatusCode = message.StatusCode.ToString(), Resources = newResources.ToArray() };
        }
    }

    public class PropfindResponseResource : IResponseResource
    {
        public string Endpoint { get; init; }

        public PropertyDictionary FoundProperties { get; init; }
    }

    public class PropertyDictionary : IReadOnlyDictionary<IDavProperty, string>
    {
        readonly IDictionary<IDavProperty, string> properties;

        public PropertyDictionary(IDictionary<IDavProperty, string> properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            this.properties = new Dictionary<IDavProperty, string>(properties, new PropertyComparer());
        }

        #region IReadOnlyDictionary members

        public string this[IDavProperty key] => properties[key];

        public IEnumerable<IDavProperty> Keys => properties.Keys;

        public IEnumerable<string> Values => properties.Values;

        public int Count => properties.Count;

        public bool ContainsKey(IDavProperty key) => properties.ContainsKey(key);

        public IEnumerator<KeyValuePair<IDavProperty, string>> GetEnumerator() => properties.GetEnumerator();

        public bool TryGetValue(IDavProperty key, [MaybeNullWhen(false)] out string value) => properties.TryGetValue(key, out value);


        IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();

        #endregion

        class PropertyComparer : IEqualityComparer<IDavProperty>
        {
            public bool Equals(IDavProperty x, IDavProperty y) => x.Name == y.Name && x.Namespace == y.Namespace;


            public int GetHashCode([DisallowNull] IDavProperty obj) => $"{obj.Namespace}:{obj.Name}".GetHashCode();

        }
    }
}

