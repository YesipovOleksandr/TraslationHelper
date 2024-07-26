using TraslationHelper.Domain.Abstract.Services;

namespace TraslationHelper.BLL.Services
{
    public class StreamService: IStreamService
    {
        public async Task<string> ReadStreamContentAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<Stream> GetStreamFromStringAsync(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(content);
            await writer.FlushAsync();
            stream.Position = 0; 
            return stream;
        }
    }
}
