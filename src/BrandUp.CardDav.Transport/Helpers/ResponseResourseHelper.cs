using BrandUp.CardDav.Server.Abstractions;
using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Models;
using BrandUp.CardDav.Transport.Models.Properties;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BrandUp.CardDav.Transport.Helpers
{
    internal static class ResponseResourseHelper
    {
        readonly static IDictionary<IDavProperty, string> propNameTable;
        readonly static PropertyInfo[] properties;

        static ResponseResourseHelper()
        {
            propNameTable = new Dictionary<IDavProperty, string>(new PropertyComparer())
            {
                {Prop.ETag , "ETag"},
                {new AddressData(), "RawVCard" },
                {Prop.CTag, "CTag" },
            };

            properties = typeof(User).GetProperties()
                .Concat(typeof(AddressBook).GetProperties())
                .Concat(typeof(Contact).GetProperties())
                .Concat(typeof(CTag).GetProperties())
                .Concat(typeof(SyncToken).GetProperties())
                .ToArray();
        }

        public static Tuple<PropertyDictionary, List<IDavProperty>> GeneratePropfindResource(IDavDocument document, IEnumerable<IDavProperty> davProperties, bool all = false)
        {
            var propertyDictionary = new Dictionary<IDavProperty, string>();
            List<IDavProperty> notFound = new();

            if (all)
            {
                SetPropValues(document, propNameTable.Keys, propertyDictionary, notFound);
            }
            else
                SetPropValues(document, davProperties, propertyDictionary, notFound);

            return new(new PropertyDictionary(propertyDictionary), notFound);

        }

        #region Helpers

        private static void SetPropValues(IDavDocument document,
            IEnumerable<IDavProperty> collection,
            IDictionary<IDavProperty, string> found,
            IList<IDavProperty> notFound)
        {

            foreach (var davProperty in collection)
            {
                try
                {
                    var value = GetValueByDavProp(document, davProperty);

                    if (value == null)
                        notFound.Add(davProperty);
                    else if (!found.TryAdd(davProperty, value))
                        continue;
                }
                catch (TargetException)
                {
                    continue;
                }
            }
        }

        private static string GetValueByDavProp(IDavDocument document, IDavProperty davProperty)
        {
            var documentProps = document.GetType().GetProperties();

            if (propNameTable.TryGetValue(davProperty, out var propName))
            {
                var property = GetPropertyByName(propName);

                if (!documentProps.Contains(property, new PropertyInfoComparer()))
                    return null;

                var converter = TypeDescriptor.GetConverter(property.PropertyType);

                return converter.ConvertToString(property.GetValue(document));
            }
            else
            {
                throw new ArgumentException("Unsupported property.");
            }
        }

        static PropertyInfo GetPropertyByName(string name)
        {
            return properties.Where(_ => _.Name == name).Single();
        }

        private class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
        {
            public bool Equals(PropertyInfo x, PropertyInfo y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode([DisallowNull] PropertyInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #endregion
    }
}
