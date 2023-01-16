using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Abstract.Responces;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Xml
{
    public static class CustomSerializer
    {
        readonly static IEnumerable<ConstructorInfo> constructors;

        static CustomSerializer()
        {
            constructors = CreateRequestConstructors();
        }

        public static IResponseBody DeserializeResponse<T>(Stream body) where T : class, IResponseBody, new()
        {
            using var reader = XmlReader.Create(body, new XmlReaderSettings { Async = true });

            var serializer = new XmlSerializer(typeof(T));
            var response = (IResponseBody)serializer.Deserialize(reader);

            return response;
        }

        public static void SerializeResponse(Stream stream, IResponseBody body)
        {
            Serialize(stream, body);
        }

        public static async Task<IResponseCreator> DeserializeRequestAsync(Stream body)
        {
            using var reader = XmlReader.Create(body, new XmlReaderSettings { Async = true });

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var name = reader.LocalName;
                    var ns = reader.NamespaceURI;

                    foreach (var constructor in constructors)
                    {
                        var instance = constructor.Invoke(new object[0]) as IRequestBody;
                        if (instance == null)
                            throw new Exception("Constructor type must be assingnable to IRequestBody");
                        if (instance.Namespace == ns && instance.Name == name)
                        {
                            instance.ReadXml(reader);
                            if (instance.GetType().IsAssignableTo(typeof(IResponseCreator)))
                                return instance as IResponseCreator;
                            else
                                throw new ArgumentException("Instance type must be assingnable to IResponseCreator");
                        }
                        else continue;
                    }

                    throw new ArgumentException($"Unknown xml property {ns}:{name}");
                }
            }

            return null;
        }

        public static void SerializeRequest(Stream stream, IRequestBody body)
        {
            Serialize(stream, body);
            stream.Position = 0;
        }

        #region Helpers 

        static void Serialize(Stream stream, object body)
        {
            var serializer = new XmlSerializer(body.GetType());

            serializer.Serialize(stream, body);
        }

        static ConstructorInfo[] CreateRequestConstructors()
        {
            var constructorsList = new List<ConstructorInfo>();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => typeof(IRequestBody).IsAssignableFrom(p) && p.IsClass);

            foreach (var type in types)
            {
                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                    constructorsList.Add(constructor);
                else
                    throw new NotSupportedException("Type must have parametless constructor");
            }

            if (!constructorsList.Any())
                throw new Exception("No constructors!");

            return constructorsList.ToArray();
        }

        #endregion

    }
}
