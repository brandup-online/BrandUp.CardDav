namespace BrandUp.Carddav.Client.Helpers
{
    public static class XmlQueryHelper
    {
        public static string Propfind(params string[] props)
            => "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                " <D:propfind xmlns:D=\"DAV:\">\r\n " +
                   " <D:prop>\r\n  " +
                            Inner(props) +
                   " </D:prop>\r\n" +
                " </D:propfind>\r\n";

        public static string AddressCollection(params string[] vCardParameters)
            => "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
            "<C:addressbook-query xmlns:D=\"DAV:\" xmlns:C=\"urn:ietf:params:xml:ns:carddav\">\n   " +
            "  <D:prop>\n  " +
            "     <D:getetag/>\n     " +
            "     <C:address-data>\n  " +
                        VCards(vCardParameters) +
            "     </C:address-data>\n " +
            "    </D:prop>\n    " +
            "<C:filter>\r\n    <C:prop-filter name=\"FN\">\r\n    </C:prop-filter>    \r\n</C:filter>" +
            "</C:addressbook-query>";

        public static string SyncCollection(string token)
            => " <?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n " +
                "  <D:sync-collection xmlns:D=\"DAV:\">\r\n   " +
                $"   <D:sync-token>{token}</D:sync-token>\r\n   " +
                    "<D:sync-level>1</D:sync-level>\r\n" +
                    "  <D:prop>\r\n    " +
                        " <D:displayname />\r\n     " +
                        "<D:current-user-principal />\r\n    " +
                        " <D:principal-URL />\r\n " +
                    " </D:prop>\r\n" +
                " </D:sync-collection>\r\n";

        #region Helpers

        private static string Inner(string[] props)
        {
            var inner = string.Empty;

            foreach (var prop in props)
            {
                inner += "<D:" + prop + "/>\r\n";
            }

            return inner;
        }

        private static string VCards(string[] props)
        {
            var inner = string.Empty;

            foreach (var prop in props)
            {
                inner += "<D:prop name=\"" + prop + "\"/>\r\n";
            }

            return inner;
        }

        #endregion
    }
}
