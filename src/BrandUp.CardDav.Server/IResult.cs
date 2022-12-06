namespace BrandUp.CardDav.Server.Abstractions
{
    public interface IResult
    {
        public bool IsSuccess { get; }
        public bool IsError { get; }
        public string ErrorMessage { get; }
    }
}
