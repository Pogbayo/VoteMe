namespace VoteMe.Domain.Exceptions
{

    public class NullException : Exception
    {
        public NullException(string message) : base(message) { }
    }
}
