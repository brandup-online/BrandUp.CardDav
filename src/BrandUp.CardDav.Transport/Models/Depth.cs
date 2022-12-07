namespace BrandUp.CardDav.Transport.Models
{
    public class Depth
    {
        public string Value { get; init; }
        private Depth(string @value)
        {
            Value = @value ?? throw new ArgumentNullException(nameof(@value));
        }

        public static Depth Zero => new("0");
        public static Depth One => new("1");
        public static Depth Infinity => new("infinity");
    }
}
