namespace Backend.CustomExceptions
{
    public class EmailNotConfirmedException : Exception
    {
        public EmailNotConfirmedException(string message) : base(message) { }
    }
}
