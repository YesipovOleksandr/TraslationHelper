using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
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

        public async Task<DocsService> GetDocsByIdAsync(string documentId)
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = _googleDocumentSettings.ClientId,
                ClientSecret = _googleDocumentSettings.ClientSecret
            };

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(_googleDocumentSettings.TokenPath, true)
            );

            var service = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _googleDocumentSettings.ApplicationName,
            });

            return service;
        }
    }
}
