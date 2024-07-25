using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;

namespace TraslationHelper.Domain.Abstract.Repositories
{
    public interface IGoogleDocsRepository
    {
        public Task<Document> GetDocsByIdAsync(string documentId);
    }
}
