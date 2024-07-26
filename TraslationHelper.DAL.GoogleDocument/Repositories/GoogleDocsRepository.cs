using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using TraslationHelper.Domain.Abstract.Repositories;
using TraslationHelper.Domain.Models.Settings;

namespace TraslationHelper.DAL.GoogleDocument.Repositories
{
    public class GoogleDocsRepository : IGoogleDocsRepository
    {
        private readonly string[] Scopes = { DocsService.Scope.DocumentsReadonly };

        private readonly GoogleDocumentSettings _googleDocumentSettings;
        public GoogleDocsRepository(IConfiguration configuration)
        {
            _googleDocumentSettings = configuration.GetSection("GoogleDocumentSettings").Get<GoogleDocumentSettings>();
        }
        public async Task<Document> GetDocsByIdAsync(string documentId)
        {
            GoogleCredential credential;
            using (var stream = new FileStream("json_google_auth.json.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DocsService.Scope.Documents);
            }

            var service = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _googleDocumentSettings.ApplicationName,
            });

            var request = service.Documents.Get(documentId);
            var document = await request.ExecuteAsync();

            return document;
        }
    }
}
