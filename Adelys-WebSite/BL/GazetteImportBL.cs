using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.BL
{
    /// <summary>
    /// Reprend les numéros qui étaient dans wwwroot. Les fichiers sont déplacés vers le stockage,
    /// l'import ne se relance donc pas et un numéro supprimé ne revient pas
    /// </summary>
    public class GazetteImportBL : IGazetteImportBL
    {
        private static readonly (string FileName, string Title, DateTime PublishedOn)[] ExistingGazettes =
        {
            ("Gazette_6_11_2022", "Gazette de novembre 2022", new DateTime(2022, 11, 6)),
            ("Gazette_christmas_2022", "Gazette spéciale Noël 2022", new DateTime(2022, 12, 15)),
            ("Gazette_26_02_2023", "Gazette de février 2023", new DateTime(2023, 2, 26))
        };

        private readonly IGazetteBL _gazetteBL;

        public GazetteImportBL(IGazetteBL gazetteBL)
        {
            _gazetteBL = gazetteBL;
        }

        public void ImportExistingGazettes(string sourceFolder)
        {
            foreach ((string fileName, string title, DateTime publishedOn) in ExistingGazettes)
            {
                string pdfPath = Path.Combine(sourceFolder, fileName + ".pdf");
                string coverPath = Path.Combine(sourceFolder, fileName + ".jpg");
                if (File.Exists(pdfPath))
                {
                    _gazetteBL.ImportGazette(pdfPath, File.Exists(coverPath) ? coverPath : null, title, publishedOn);
                    File.Delete(pdfPath);
                    File.Delete(coverPath);
                }
            }
        }
    }
}
