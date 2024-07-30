using TraslationHelper.Domain.Abstract.Repositories;
using TraslationHelper.Domain.Abstract.Services;

namespace TraslationHelper.BLL.Services
{
    public class GoogleDocTranslationService : IGoogleDocTranslationService
    {
        private readonly IGoogleDocsRepository _googleDocsRepository;

        public GoogleDocTranslationService(IGoogleDocsRepository googleDocsRepository)
        {
            _googleDocsRepository = googleDocsRepository;
        }

        public async Task<Dictionary<string, string>> ExtractTranslationsAsync(string documentId)
        {
            var document = await _googleDocsRepository.GetDocsByIdAsync(documentId);

            var dictionary = new Dictionary<string, string>();
            foreach (var element in document.Body.Content)
            {
                if (element.Table != null)
                {
                    foreach (var row in element.Table.TableRows)
                    {
                        List<string> text1 = new List<string>();
                        List<string> text2 = new List<string>();

                        var TableCells1 = row.TableCells[0];
                        foreach (var elements1 in TableCells1?.Content)
                        {
                            foreach (var item1 in elements1?.Paragraph?.Elements)
                            {
                                if (!string.IsNullOrWhiteSpace(item1.TextRun?.Content))
                                {
                                    text1.Add(item1.TextRun.Content.Trim());
                                }
                            }
                        }

                        var TableCells2 = row.TableCells[1];
                        foreach (var elements2 in TableCells2?.Content)
                        {
                            foreach (var item2 in elements2?.Paragraph?.Elements)
                            {
                                if (!string.IsNullOrWhiteSpace(item2.TextRun?.Content))
                                {
                                    text2.Add(item2.TextRun.Content.Trim());
                                }
                            }
                        }

                        int count = Math.Min(text1.Count, text2.Count);

                        for (int i = 0, j = 0; i < count; i++, j++)
                        {
                            if (i < text1.Count() && j < text2.Count())
                            {
                                string key = text1[i].Trim();
                                string value = text2[j].Trim();

                                if (value.Length < 3 && text1.Count < text2.Count)
                                {
                                    j++;
                                    value = text2[j].Trim();
                                }

                                char[] unwantedChars = { ',', ':', '.', '-', '—', ' ', '\t' };

                                while (key.Length > 0 && unwantedChars.Contains(key[0]))
                                {
                                    key = key.Substring(1).TrimStart(unwantedChars);
                                }

                                while (value.Length > 0 && unwantedChars.Contains(value[0]))
                                {
                                    value = value.Substring(1).TrimStart(unwantedChars);
                                }

                                if (!string.IsNullOrEmpty(key) && !dictionary.ContainsKey(key.ToLower()))
                                {
                                    dictionary.Add(key.ToLower(), value);
                                }
                            }
                        }
                    }
                }
            }

            var sortedDict = dictionary.OrderByDescending(kvp => kvp.Key.Length)
                                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return sortedDict;
        }
    }
}
