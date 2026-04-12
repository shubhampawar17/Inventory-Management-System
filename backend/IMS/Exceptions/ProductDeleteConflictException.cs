namespace IMS.Exceptions
{
    public class ProductDeleteConflictException : Exception
    {
        public ProductDeleteConflictException(string message) : base(message) { }
    }
}
