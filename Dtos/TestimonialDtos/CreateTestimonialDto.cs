using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos
{
    public class CreateTestimonialDto
    {
        [Required(ErrorMessage = "Ad soyad gereklidir.")]
        [StringLength(150, ErrorMessage = "Ad soyad en fazla 150 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string NameSurname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlık gereklidir.")]
        [StringLength(150, ErrorMessage = "Başlık en fazla 150 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Görsel URL gereklidir.")]
        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        [Display(Name = "Görsel URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yorum gereklidir.")]
        [StringLength(1500, ErrorMessage = "Yorum en fazla 1500 karakter olabilir.")]
        [Display(Name = "Yorum")]
        public string ReviewDetail { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        [Display(Name = "Puan")]
        public int ReviewScore { get; set; }

        [Display(Name = "Durum")]
        public bool Status { get; set; }
    }
}
