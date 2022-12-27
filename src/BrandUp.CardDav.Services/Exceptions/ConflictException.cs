namespace BrandUp.CardDav.Services.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(object obj, string name) : base($"The collections {obj.GetType().Name} already have object with name {name}")
        { }
    }
}
