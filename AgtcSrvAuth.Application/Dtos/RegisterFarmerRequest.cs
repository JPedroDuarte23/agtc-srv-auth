using System.ComponentModel.DataAnnotations;

namespace AgtcSrvAuth.Application.Dtos;
public class RegisterFarmerRequest {
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Favor informar um endereço de e-mail válido.")]
    [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).+$",
    ErrorMessage = "A senha deve conter ao menos uma letra, um número e um caractere especial.")]
    public string Password { get; set; }

    public RegisterFarmerRequest(string name, string email, string password) {
        Name = name;
        Email = email;  
        Password = password;
    }
}
