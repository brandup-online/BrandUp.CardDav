using BrandUp.Carddav.Client.Models;
using BrandUp.Carddav.Client.Models.Responses;
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
            CarddavResponse response = new();

            var mainNode = xmlDocument["multistatus", "DAV:"];
            var nsmgr = new XmlNamespaceManager(mainNode.CreateNavigator().NameTable);
            var d = mainNode.GetPrefixOfNamespace(mainNode.NamespaceURI);

            nsmgr.AddNamespace(d, "DAV:");
            nsmgr.AddNamespace("e", "urn:ietf:params:xml:ns:carddav");

            foreach (XmlNode node in xmlDocument["multistatus", "DAV:"].ChildNodes)
            {
                var href = node["href"]?.InnerText;
                var eTag = node.SelectSingleNode($"{d}:propstat/{d}:prop/{d}:getetag", nsmgr)?.InnerText;
                if (node.Name.Contains("sync-token"))
                {
                    response.SyncToken = node.Name;
                    continue;
                }
                if (node.SelectSingleNode($"{d}:propstat/{d}:prop/{d}:resourcetype", nsmgr).HasChildNodes)
                {
                    //address book 
                    var name = node.SelectSingleNode($"{d}:propstat/{d}:prop/{d}:displayname", nsmgr).InnerText;
                    response.addressBooks.Add(new AddressBookResponse { Etag = eTag, Endpoint = href, DisplayName = name });
                }
                else
                {
                    //vcard

                    response.vCardLinks.Add(new VCardResponse { Etag = eTag, Endpoint = href });
                }
            }

            return response;
        }
    }
}
