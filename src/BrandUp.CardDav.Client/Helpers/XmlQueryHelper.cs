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

        public static string AddressCollection()
            => "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
            "<C:addressbook-query xmlns:D=\"DAV:\" xmlns:C=\"urn:ietf:params:xml:ns:carddav\">\n   " +
            "  <D:prop>\n  " +
            "     <D:getetag/>\n     " +
            "   <C:address-data>\n      " +
            "     </C:address-data>\n " +
            "    </D:prop>\n    " +
            "<C:filter>\r\n    <C:prop-filter name=\"FN\">\r\n    </C:prop-filter>    \r\n</C:filter>" +
            "</C:addressbook-query>";

        public static string SyncCollection()
            => " <?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n " +
                "  <D:sync-collection xmlns:D=\"DAV:\">\r\n   " +
                "  <D:sync-token/>\r\n   " +
                "  <D:sync-level>1</D:sync-level>\r\n" +
                "  <D:prop xmlns:R=\"urn:ns.example.com:boxschema\">\r\n       " +
                "<D:getetag/>\r\n       " +
                "<R:bigbox/>\r\n    " +
                " </D:prop>\r\n  " +
                " </D:sync-collection>";

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

        #endregion
    }
}
