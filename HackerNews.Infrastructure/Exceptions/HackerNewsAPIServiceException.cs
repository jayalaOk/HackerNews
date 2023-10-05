namespace HackerNews.Infrastructure.Exceptions
{
    public class HackerNewsAPIServiceException : Exception
    {
        public HackerNewsAPIServiceException(string message) : base(message)
        {
        }
    }

}
