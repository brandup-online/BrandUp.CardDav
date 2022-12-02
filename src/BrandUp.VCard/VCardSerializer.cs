namespace BrandUp.VCard
{
    public static class VCardSerializer
    {
        public static async Task<string> SerializeAsync(VCardModel vCard, CancellationToken cancellationToken)
        {
            var result = "BEGIN:VCARD\r\nVERSION:3.0\r\n";

            result += vCard.UId == null ? "" : "UID:" + vCard.UId + "\r\n";

            result += "N:" + string.Join(";", string.Join(",", vCard.Name.FamilyNames),
                                       string.Join(",", vCard.Name.GivenNames),
                                       string.Join(",", vCard.Name.AdditionalNames),
                                       string.Join(",", vCard.Name.HonorificPrefixes),
                                       string.Join(",", vCard.Name.HonorificSuffixes)) + "\r\n";

            result += $"FN:{vCard.FormattedName}\r\n";

            result += await Task.Run(() =>
            {
                var result = "";
                foreach (var item in vCard.AdditionalFields)
                {
                    result += item.Key + ":" + item.Value + "\r\n";
                }

                return result;
            }, cancellationToken);

            result += await Task.Run(() =>
            {
                var result = "";
                foreach (var item in vCard.Emails)
                {
                    var emailStr = "EMAIL";

                    if (item.Kind != null)
                        emailStr += $";TYPE={item.Kind}";

                    if (item.Types.Any())
                        emailStr += ";" + string.Join(";", item.Types.Select(t => $"TYPE={t}").ToList());

                    emailStr += $":{item.Email}\r\n";

                    result += emailStr;
                }

                return result;
            }, cancellationToken);

            result += await Task.Run(() =>
            {
                var result = "";
                foreach (var item in vCard.Phones)
                {
                    var phoneStr = "TEL";

                    if (item.Kind != null)
                        phoneStr += $";TYPE={item.Kind}";
                    if (item.Types.Any())
                        phoneStr += ";" + string.Join(";", item.Types.Select(t => $"TYPE={t}").ToList());

                    phoneStr += $":{item.Phone}\r\n";

                    result += phoneStr;
                }

                return result;
            }, cancellationToken);

            result += "END:VCARD\r\n";

            return result;
        }
    }
}
