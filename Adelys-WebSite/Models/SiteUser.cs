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
        public const string Redacteur = "Redacteur";
        public const string GazetteEditors = Admin + "," + Redacteur;
        public static readonly string[] All = { Admin, Redacteur };
    }
}
