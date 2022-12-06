using BrandUp.CardDav.Client.Models;
using BrandUp.CardDav.Client.Models.Responses;
using BrandUp.VCard;
using System.Xml;

namespace BrandUp.CardDav.Client.Xml
{
    internal static class XmlParser
    {
        public static CarddavContent GenerateCarddavContent(Stream xmlStream, bool closeStream = true)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlStream);

            if (closeStream)
                xmlStream.Dispose();

            CarddavContent response = new() { RawXml = xmlDocument.OuterXml };

            foreach (XmlNode responseNode in xmlDocument["multistatus", "DAV:"].ChildNodes)
            {
                var href = responseNode["href"]?.InnerText ?? responseNode["href", "DAV:"]?.InnerText;

                var propNode = responseNode["propstat", "DAV:"]["prop", "DAV:"];
                var eTag = propNode["getetag", "DAV:"]?.InnerText;
                var cTag = propNode["getctag", "http://calendarserver.org/ns/"]?.InnerText;

                if (propNode["sync-token", "DAV:"] != null)
                {
                    response.SyncToken = propNode["sync-token", "DAV:"].InnerText;
                }
                if (propNode["address-data", "urn:ietf:params:xml:ns:carddav"] != null)
                {
                    //vcard data

                    var vCard = propNode["address-data", "urn:ietf:params:xml:ns:carddav"].InnerText;
                    response.VCardResponse.Add(new() { Etag = eTag, Ctag = cTag, Endpoint = href, VCard = VCardParser.Parse(vCard) });
                }
                if (propNode["resourcetype", "DAV:"] != null && propNode["resourcetype", "DAV:"].InnerXml != "")
                {
                    //address book

                    var name = propNode["resourcetype", "DAV:"]?.InnerText;
                    response.AddressBooks.Add(new AddressBookResponse { Etag = eTag, Ctag = cTag, Endpoint = href, DisplayName = name });
                }
                else
                {
                    response.ResourceEndpoints.Add(new ResourceResponse { Etag = eTag, Ctag = cTag, Endpoint = href });
                }
            }

            return response;
        }
    }
}
