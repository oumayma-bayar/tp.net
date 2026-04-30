using System.ComponentModel.DataAnnotations;

namespace DashboardData.Models
{
    public class SignUp
    {
        [Required(ErrorMessage = "L'adresse email est requise.")]
        [EmailAddress(ErrorMessage = "Veuillez entrer une adresse email valide.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre 6 et 100 caractères.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "La confirmation du mot de passe est requise.")]
        [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; } = "";
    }
}