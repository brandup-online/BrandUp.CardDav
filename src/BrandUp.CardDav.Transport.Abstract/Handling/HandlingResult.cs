namespace BrandUp.CardDav.Transport.Abstract.Handling
{
    public class HandlingResult
    {
        public bool IsFound { get; set; }
        public string Value { get; set; }

        public static HandlingResult NotFound => new() { IsFound = false, Value = null };
        public static HandlingResult Found(string value) => new() { IsFound = true, Value = value };
    }
}
