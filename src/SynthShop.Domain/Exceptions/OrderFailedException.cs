namespace SynthShop.Domain.Exceptions
{
    public class OrderFailedException : Exception
    {
        public OrderFailedException()
        { }

        public OrderFailedException(string message)
            : base(message)
        { }

        public OrderFailedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
