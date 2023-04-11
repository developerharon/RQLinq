namespace RQLinq
{
    public class RqlTokenException : Exception
    {
        public RqlTokenException() { }

        public RqlTokenException(string message) : base(message) { }

        public RqlTokenException(IEnumerable<string> diagnostics)
        {

        }
    }
}