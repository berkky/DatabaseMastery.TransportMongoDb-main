using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Dtos.BrandDtos
{
    public class CreateBrandDto
    {
        [Required(ErrorMessage = "Marka adı gereklidir.")]
        [StringLength(150, ErrorMessage = "Marka adı en fazla 150 karakter olabilir.")]
        [Display(Name = "Marka Adı")]
        public string BrandName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Görsel URL gereklidir.")]
        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        [Display(Name = "Görsel URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Aktif Durum")]
        public bool IsStatus { get; set; }
    }
}
