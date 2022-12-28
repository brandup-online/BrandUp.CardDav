using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Server.Tests
{
    internal static class TestVCards
    {
        public static string VCard1String => "BEGIN:VCARD\r\n" +
                                            "VERSION:3.0\r\n" +
                                            "N:Doe;John;;;\r\n" +
                                            "FN:John Doe\r\n" +
                                            "ORG:Example.com Inc.;\r\n" +
                                            "TITLE:Imaginary test person\r\n" +
                                            "EMAIL;type=WORK;type=INTERNET;type=pref:johnDoe@example.org\r\n" +
                                            "TEL;type=WORK;type=pref:+1 617 555 1212\r\n" +
                                            "TEL;type=WORK:+1 (617) 555-1234\r\n" +
                                            "TEL;type=CELL:+1 781 555 1212\r\n" +
                                            "TEL;type=HOME:+1 202 555 1212\r\n" +
                                            "END:VCARD\r\n";

        public static VCardModel VCard1 => VCardParser.Parse(VCard1String);

        public static string VCard2String => "BEGIN:VCARD\r\n" +
                                            "VERSION:3.0\r\n" +
                                            "N:Die;Jahn;;;\r\n" +
                                            "FN:Jahn Die\r\n" +
                                            "ORG:Example.net Inc.;\r\n" +
                                            "TITLE:Imaginary test person\r\n" +
                                            "EMAIL;type=WORK;type=INTERNET;type=pref:jahnDie@example.org\r\n" +
                                            "TEL;type=WORK;type=pref:+1 627 555 1212\r\n" +
                                            "TEL;type=WORK:+1 (617) 535-1234\r\n" +
                                            "TEL;type=CELL:+1 781 555 1242\r\n" +
                                            "TEL;type=HOME:+1 202 555 1211\r\n" +
                                            "END:VCARD\r\n";

        public static VCardModel VCard2 => VCardParser.Parse(VCard2String);

        public static string VCard3String => "BEGIN:VCARD\r\n" +
                                            "VERSION:3.0\r\n" +
                                            "N:Sha;Di;;;\r\n" +
                                            "FN:Di Sha\r\n" +
                                            "ORG:Example.com Inc.;\r\n" +
                                            "TITLE:Imaginary test person\r\n" +
                                            "EMAIL;type=WORK;type=INTERNET;type=pref:milo@example.org\r\n" +
                                            "TEL;type=WORK;type=pref:+1 232 555 1212\r\n" +
                                            "TEL;type=WORK:+1 (617) 666-1234\r\n" +
                                            "TEL;type=CELL:+1 781 777 1212\r\n" +
                                            "TEL;type=HOME:+1 202 113 2112\r\n" +
                                            "END:VCARD\r\n";

        public static VCardModel VCard3 => VCardParser.Parse(VCard3String);
    }
}
