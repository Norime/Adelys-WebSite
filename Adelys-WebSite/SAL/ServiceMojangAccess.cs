using Adelys_WebSite.SAL.Interface;

namespace Adelys_WebSite.SAL
{
    public class ServiceMojangAccess : IServiceMojangAccess
    {
        private static HttpClient client = new HttpClient();

        public async Task<string> GetSkinData(string uuid)
        {
            string apiUrl = $"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return json;
                }
                else
                {
                    // Gérer les erreurs de requête
                    throw new Exception($"Erreur HTTP: {response.StatusCode}");

                }
            }
            catch (Exception ex)
            {
                // Gérer les erreurs d'exception
                throw new Exception($"Erreur lors de la requête API: {ex.Message}");
            }
        }
    }
}
