using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Dtos.SliderDto
{
    public class CreateSliderDto
    {
        [Required(ErrorMessage = "Başlık gereklidir.")]
        [StringLength(150, ErrorMessage = "Başlık en fazla 150 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string SliderTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alt başlık gereklidir.")]
        [StringLength(200, ErrorMessage = "Alt başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Alt Başlık")]
        public string Subtitle { get; set; } = string.Empty;

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
