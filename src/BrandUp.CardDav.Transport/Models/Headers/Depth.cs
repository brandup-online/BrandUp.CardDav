namespace BrandUp.CardDav.Transport.Models.Headers
{
    /// <summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc4918#section-10.2"/>
    /// </summary>
    public class Depth
    {
        /// <summary>
        /// string value
        /// </summary>
        public string Value { get; init; }
        private Depth(string @value)
        {
            Value = @value ?? throw new ArgumentNullException(nameof(@value));
        }

        /// <summary>
        /// 
        /// </summary>
        public static Depth Zero => new("0");

        /// <summary>
        /// 
        /// </summary>
        public static Depth One => new("1");

        /// <summary>
        /// 
        /// </summary>
        public static Depth Infinity => new("infinity");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Depth Parse(string s)
        {
            return s switch
            {
                "0" => Zero,
                "1" => One,
                "infinity" => Infinity,
                _ => throw new ArgumentException(nameof(s))
            };

        }
    }
}
