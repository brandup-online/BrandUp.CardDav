using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Abstract.Responces;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public static class CustomSerializer
    {
        readonly static IEnumerable<ConstructorInfo> constructors;

        static CustomSerializer()
        {
            constructors = CreateRequestConstructors();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <returns></returns>
        public static IResponseBody DeserializeResponse<T>(Stream body) where T : class, IResponseBody, new()
        {
            using var reader = XmlReader.Create(body, new XmlReaderSettings { Async = true });

            var serializer = new XmlSerializer(typeof(T));
            var response = (IResponseBody)serializer.Deserialize(reader);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="body"></param>
        public static void SerializeResponse(Stream stream, IResponseBody body)
        {
            Serialize(stream, body);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<IRequestBody> DeserializeRequestAsync(Stream body)
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
                            if (instance.GetType().IsAssignableTo(typeof(IBodyWithFilter)))
                                return instance;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="body"></param>
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

            var types = Assembly.GetExecutingAssembly().GetTypes();

            if (!types.Any())
                throw new Exception("No types in assembly");

            types = types.Where(p => typeof(IRequestBody).IsAssignableFrom(p) && p.IsClass).ToArray();
            if (!types.Any())
                throw new Exception("No types assignable to IRequestBody in assembly");

            foreach (var type in types)
            {
                var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
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
