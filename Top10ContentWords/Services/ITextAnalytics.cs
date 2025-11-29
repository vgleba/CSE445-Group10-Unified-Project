using System.ServiceModel;

namespace Top10ContentWords.Services
{
    [ServiceContract]
    public interface ITextAnalytics
    {
        /// <summary>
        /// Returns top 10 content words.
        /// If inputOrUrl is an absolute http/https URL, the service fetches text via WebDownload (?raw=false).
        /// Otherwise, it's treated as raw text (HTML allowed).
        /// </summary>
        [OperationContract]
        string[] Top10ContentWords(string inputOrUrl);
    }
}
