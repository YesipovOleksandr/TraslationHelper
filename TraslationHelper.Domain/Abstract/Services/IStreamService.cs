namespace TraslationHelper.Domain.Abstract.Services
{
    public interface IStreamService
    {
        public Task<string> ReadStreamContentAsync(Stream stream);
        public Task<Stream> GetStreamFromStringAsync(string content);
    }
}
