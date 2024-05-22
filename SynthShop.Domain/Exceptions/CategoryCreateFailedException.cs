namespace SynthShop.Domain.Exceptions
{
    public class CategoryCreateFailedException : Exception
    {
        public CategoryCreateFailedException()
        { }

        public CategoryCreateFailedException(string message)
            : base(message)
        { }

        public CategoryCreateFailedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
