using BrandUp.Carddav.Client.Models;
using BrandUp.Carddav.Client.Models.Responses;
using BrandUp.Carddav.Client.Parsers;
using System.Xml;

namespace BrandUp.Carddav.Client.Xml
{
    internal class XmlParser
    {
        readonly XmlDocument xmlDocument;
        public XmlParser(Stream xmlStream)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlStream);

            xmlStream.Dispose();
        }

        public CarddavResponse GenerateCarddavResponse()
        {
            CarddavResponse response = new() { RawXml = xmlDocument.OuterXml };


            var mainNode = xmlDocument["multistatus", "DAV:"];
            var nsmgr = new XmlNamespaceManager(mainNode.CreateNavigator().NameTable);
            var d = mainNode.GetPrefixOfNamespace(mainNode.NamespaceURI);

            nsmgr.AddNamespace(d, "DAV:");

            foreach (XmlNode responseNode in xmlDocument["multistatus", "DAV:"].ChildNodes)
            {
                var href = responseNode["href"]?.InnerText ?? responseNode["href", "DAV:"]?.InnerText;
                var eTag = responseNode.SelectSingleNode($"{d}:propstat/{d}:prop/{d}:getetag", nsmgr)?.InnerText;
                if (responseNode.Name.Contains("sync-token"))
                {
                    response.SyncToken = responseNode.Name;
                    continue;
                }
                var propNode = responseNode.SelectSingleNode($"{d}:propstat/{d}:prop", nsmgr);
                if (propNode["address-data", "urn:ietf:params:xml:ns:carddav"] != null)
                {
                    //vcard data

                    var vCard = responseNode.SelectSingleNode($"{d}:propstat/{d}:prop", nsmgr)["address-data", "urn:ietf:params:xml:ns:carddav"].InnerText;
                    response.VCardResponse.Add(new() { Etag = eTag, Endpoint = href, VCard = VCardParser.Parse(vCard) });
                }
                if (propNode["resourcetype", "DAV:"] != null && propNode["resourcetype", "DAV:"].InnerXml != "")
                {
                    //address book

                    var name = propNode["resourcetype", "DAV:"]?.InnerText;
                    response.AddressBooks.Add(new AddressBookResponse { Etag = eTag, Endpoint = href, DisplayName = name });

                }
                else
                {
                    response.ResourceEndpoints.Add(new ResourceResponse { Etag = eTag, Endpoint = href });
                }
            }

            return response;
        }
    }
}
