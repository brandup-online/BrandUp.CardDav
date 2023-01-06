using System.Text.RegularExpressions;

namespace BrandUp.CardDav.VCard
{
    public static class VCardParser
    {
        readonly static CardProperty[] mustSingle = new CardProperty[]
        {
             CardProperty.KIND,
             CardProperty.N,
             CardProperty.BDAY,
             CardProperty.ANNIVERSARY,
             CardProperty.GENDER,
             CardProperty.PRODID,
             CardProperty.REV,
             CardProperty.UID,
             CardProperty.VERSION
        };

        public static async Task<VCardModel> ParseAsync(Stream responseStream, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(responseStream);

            return new(await ParseLinesAsync(reader, cancellationToken));
        }

        public static async Task<VCardModel> ParseAsync(string vCardRaw, CancellationToken cancellationToken)
            => new(await ParseLinesAsync(new StringReader(vCardRaw), cancellationToken));

        public static VCardModel Parse(string vCardRaw)
            => new(ParseLinesAsync(new StringReader(vCardRaw), CancellationToken.None).Result);

        public static VCardModel Parse(Stream vCardStream)
            => ParseAsync(vCardStream, CancellationToken.None).Result;

        public static bool TryParse(string vCardRaw, out VCardModel vCard)
        {
            try
            {
                vCard = new(ParseLinesAsync(new StringReader(vCardRaw), CancellationToken.None).Result);

                return true;
            }
            catch
            {
                vCard = null;

                return false;
            }
        }

        #region Helpers 

        internal static async Task<Dictionary<CardProperty, IEnumerable<VCardLine>>> ParseLinesAsync(TextReader reader, CancellationToken cancellationToken)
        {
            Dictionary<CardProperty, IEnumerable<VCardLine>> vCard = new();
            while (true)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line == null)
                    break;

                if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (string.Equals(line, "END:VCARD", StringComparison.InvariantCultureIgnoreCase))
                {
                    line = await reader.ReadLineAsync(cancellationToken);
                    if (line == null)
                        return new(vCard);
                    else if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                        throw new NotSupportedException("vCard with some contacts in one file.");
                }

                AddLineToCard(vCard, line);
            }
            return new(vCard);
        }

        #endregion

        #region Helpers 

        internal static void AddLineToCard(IDictionary<CardProperty, IEnumerable<VCardLine>> card, string line, bool canReplace = false)
        {
            var split = line.Split(':');
            var property = split[0].Split(';').First();
            var parameters = split[0].Replace($"{property};", "").Split(';');
            var value = split[1];

            var parsedLine = ParseLine(value, parameters);

            if (Enum.TryParse<CardProperty>(property, true, out var cardProperty))
            {
                if (card.TryGetValue(cardProperty, out var field))
                {
                    if (!mustSingle.Contains(cardProperty))
                        card[cardProperty] = field.Append(parsedLine).ToArray();
                    else if (canReplace)
                    {
                        card[cardProperty] = new List<VCardLine> { parsedLine };
                    }
                    else throw new ArgumentException($"Incorrect vCard: field {cardProperty} must be single");
                }
                else
                {
                    card.Add(cardProperty, new List<VCardLine>() { parsedLine });
                }
            }
            else new ArgumentException(property);
        }

        internal static void ReplaceLine(IDictionary<CardProperty, IEnumerable<VCardLine>> card, string line, string oldValue)
        {
            var split = line.Split(':');
            var property = split[0].Split(';').First();
            var parameters = split[0].Replace($"{property};", "").Split(';');
            var value = split[1];

            var parsedLine = ParseLine(value, parameters);

            if (Enum.TryParse<CardProperty>(property, true, out var cardProperty))
            {
                if (card.TryGetValue(cardProperty, out var field))
                {
                    var temp = card[cardProperty];
                    temp.ToList().RemoveAll(x => x.Value == oldValue);
                    temp.Append(parsedLine);

                    card[cardProperty] = temp;
                }
                else
                {
                    card.Add(cardProperty, new List<VCardLine>() { parsedLine });
                }
            }
            else new ArgumentException(property);
        }

        private static VCardLine ParseLine(string value, string[] parameters)
        {
            var paramDict = new Dictionary<CardParameter, IEnumerable<string>>();
            foreach (var parameter in parameters)
            {
                var pair = parameter.Split('=');
                if (pair.Length > 2)
                    throw new ArgumentException("Parsing error");

                if (pair.Length < 2)
                    return new() { Value = value };

                if (Enum.TryParse<CardParameter>(pair[0], true, out var parssedParameter))
                {
                    if (paramDict.TryGetValue(parssedParameter, out var paramValue))
                    {
                        paramDict[parssedParameter] = paramValue.Append(pair[1]);
                    }
                    else
                    {
                        paramDict.Add(parssedParameter, new List<string>() { pair[1] });
                    }
                }
                else new ArgumentException(pair[0]);
            }

            return new() { Value = value, Parameters = paramDict };
        }

        #endregion
    }
}
