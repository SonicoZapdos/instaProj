using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations;

namespace instaProj.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo Telefone é obrigatório.")]

        [Phone(ErrorMessage = "O campo Telefone deve ser um número de telefone válido.")]
        public string Telefone { get; set; }

        public string? Url { get; set; }
        public string? Bio { get; set; }
        public string? PictureLocal {  get; set; }
    }
}
