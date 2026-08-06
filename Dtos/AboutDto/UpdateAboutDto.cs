using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Dtos.AboutDto
{
    public class UpdateAboutDto
    {
        [Required(ErrorMessage = "Hakkımızda kimliği gereklidir.")]
        [Display(Name = "Hakkımızda Kimliği")]
        public string AboutId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlık gereklidir.")]
        [StringLength(150, ErrorMessage = "Başlık en fazla 150 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama gereklidir.")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Görsel URL gereklidir.")]
        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        [Display(Name = "Görsel URL")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
