using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [StringLength(64, ErrorMessage = "Kullanıcı adı en fazla 64 karakter olabilir.")]
        [Display(Name = "Kullanıcı adı")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; } = string.Empty;
    }
}
