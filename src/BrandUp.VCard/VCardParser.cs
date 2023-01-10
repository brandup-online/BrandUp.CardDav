namespace BrandUp.CardDav.VCard
{
    internal static class VCardParser
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

        public static IDictionary<CardProperty, PropertyModel> Parse(TextReader reader)
        {
            Dictionary<CardProperty, PropertyModel> vCard = new();
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;

                if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (string.Equals(line, "END:VCARD", StringComparison.InvariantCultureIgnoreCase))
                {
                    line = reader.ReadLine();
                    if (line == null)
                        return vCard.ToDictionary(k => k.Key, v => new PropertyModel(v.Value));
                    else if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                        throw new NotSupportedException("vCard with some contacts in one file.");
                }

                AddLineToCard(vCard, line);
            }
            return vCard.ToDictionary(k => k.Key, v => new PropertyModel(v.Value));
        }

        #region Helpers 

        internal static void AddLineToCard(IDictionary<CardProperty, PropertyModel> card, string line, bool canReplace = false)
        {
            var split = line.Split(':');

            var value = split[1];
            var splitTypes = split[0].Split(';');

            var property = splitTypes[0];
            var parameters = splitTypes[1..];

            var parsedLine = ParseLine(value, parameters);

            if (Enum.TryParse<CardProperty>(property, true, out var cardProperty))
            {
                if (card.TryGetValue(cardProperty, out var field))
                {
                    if (!mustSingle.Contains(cardProperty))
                        field.AddLine(parsedLine);
                    else if (canReplace)
                    {
                        card[cardProperty] = new PropertyModel(parsedLine);
                    }
                    else throw new ArgumentException($"Incorrect vCard: field {cardProperty} must be single");
                }
                else
                {
                    card.Add(cardProperty, new PropertyModel(parsedLine));
                }
            }
            else new ArgumentException(property);
        }

        private static VCardLine ParseLine(string value, string[] parameters)
        {
            var vCardParameters = new List<VCardParameter>();
            foreach (var parameter in parameters)
            {
                var pair = parameter.Split('=');
                if (pair.Length > 2)
                    throw new ArgumentException("Parsing error");

                if (pair.Length < 2)
                    return new() { Value = value };

                if (Enum.TryParse<CardParameter>(pair[0], true, out var parssedParameter))
                {
                    vCardParameters.Add(new(parssedParameter, pair[1]));
                }
                else new ArgumentException(pair[0]);
            }

            return new() { Value = value, Parameters = vCardParameters };
        }

        #endregion
    }
}
