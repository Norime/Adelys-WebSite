using System.Security.Claims;

namespace Adelys_WebSite.Models
{
    public class SiteUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = SiteRoles.Redacteur;
        public bool IsActive { get; set; } = true;
        public bool MustChangePassword { get; set; } = true;
        public string SecurityStamp { get; set; } = string.Empty;
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastLoginUtc { get; set; }
    }

    public static class SiteRoles
    {
        public const string Admin = "Admin";
        /// <summary>
        /// Gère les commerces, la gazette, le règlement et les events, sans accès aux comptes ni à l'accueil
        /// </summary>
        public const string Moderateur = "Moderateur";
        public const string Redacteur = "Redacteur";
        public const string GazetteEditors = Admin + "," + Moderateur + "," + Redacteur;
        public const string ContentEditors = Admin + "," + Moderateur;
        public static readonly string[] All = { Admin, Moderateur, Redacteur };

        public static bool CanEditContent(ClaimsPrincipal user) => user.IsInRole(Admin) || user.IsInRole(Moderateur);

        public static bool CanEditGazette(ClaimsPrincipal user) => CanEditContent(user) || user.IsInRole(Redacteur);

        public static string DisplayName(string role) => role switch
        {
            Admin => "Administrateur",
            Moderateur => "Modérateur",
            _ => "Rédacteur"
        };
    }
}
