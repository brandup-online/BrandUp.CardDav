namespace BrandUp.CardDav.VCard
{
    /// <summary>
    /// 
    /// </summary>
    public class VCardParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public CardParameter Parameter { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VCardParameter(CardParameter parameter, string value)
        {
            Parameter = parameter;
            Value = value?.ToLower() ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as VCardParameter;
            if (other == null) return false;

            return Parameter == other.Parameter && Value == other.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
