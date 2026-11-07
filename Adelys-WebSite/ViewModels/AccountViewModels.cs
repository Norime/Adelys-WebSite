using Adelys_WebSite.Models;
using System.ComponentModel.DataAnnotations;

namespace Adelys_WebSite.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'identifiant est obligatoire")]
        [Display(Name = "Identifiant")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Le mot de passe actuel est obligatoire")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire")]
        [StringLength(128, MinimumLength = 12, ErrorMessage = "Le mot de passe doit faire entre 12 et 128 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare(nameof(NewPassword), ErrorMessage = "Les deux mots de passe sont différents")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "L'identifiant est obligatoire")]
        [RegularExpression("^[A-Za-z0-9_.-]{3,32}$", ErrorMessage = "L'identifiant doit faire 3 à 32 caractères : lettres, chiffres, . _ -")]
        [Display(Name = "Identifiant")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = SiteRoles.Redacteur;
    }

    public class UsersViewModel
    {
        public List<SiteUser> Users { get; set; } = new();
        public CreateUserViewModel NewUser { get; set; } = new();
        public int CurrentUserId { get; set; }
        public string? GeneratedForUsername { get; set; }
        public string? GeneratedPassword { get; set; }
    }
}
