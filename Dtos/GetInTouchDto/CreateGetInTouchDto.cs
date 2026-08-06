using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto
{
    public class CreateGetInTouchDto
    {
        [Required(ErrorMessage = "Rozet başlığı gereklidir.")]
        [StringLength(100, ErrorMessage = "Rozet başlığı en fazla 100 karakter olabilir.")]
        [Display(Name = "Rozet Başlığı")]
        public string BadgeTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ana başlık gereklidir.")]
        [StringLength(200, ErrorMessage = "Ana başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Ana Başlık")]
        public string MainTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama gereklidir.")]
        [StringLength(1500, ErrorMessage = "Açıklama en fazla 1500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "1. özellik başlığı gereklidir.")]
        [StringLength(150, ErrorMessage = "1. özellik başlığı en fazla 150 karakter olabilir.")]
        [Display(Name = "1. Özellik Başlığı")]
        public string Feature1Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "1. özellik açıklaması gereklidir.")]
        [StringLength(750, ErrorMessage = "1. özellik açıklaması en fazla 750 karakter olabilir.")]
        [Display(Name = "1. Özellik Açıklaması")]
        public string Feature1Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "2. özellik başlığı gereklidir.")]
        [StringLength(150, ErrorMessage = "2. özellik başlığı en fazla 150 karakter olabilir.")]
        [Display(Name = "2. Özellik Başlığı")]
        public string Feature2Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "2. özellik açıklaması gereklidir.")]
        [StringLength(750, ErrorMessage = "2. özellik açıklaması en fazla 750 karakter olabilir.")]
        [Display(Name = "2. Özellik Açıklaması")]
        public string Feature2Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Görsel URL gereklidir.")]
        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        [Display(Name = "Görsel URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Durum")]
        public bool Status { get; set; }
    }
}
