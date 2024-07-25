using Google.Apis.Docs.v1;

namespace TraslationHelper.Domain.Abstract.Repositories
{
    public interface IGoogleDocsRepository
    {
        public Task<DocsService> GetDocsByIdAsync(string documentId);
    }
}
