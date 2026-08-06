using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Dtos.ShipmentTrackingDtos
{
    public class UpdateShipmentTrackingDto
    {
        [Required(ErrorMessage = "Takip numarası gereklidir.")]
        [StringLength(64, ErrorMessage = "Takip numarası en fazla 64 karakter olabilir.")]
        [Display(Name = "Takip Numarası")]
        public string TrackingNumber { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Geçersiz hareket sırası.")]
        [Display(Name = "Hareket Sırası")]
        public int TrackingIndex { get; set; }

        [Display(Name = "Hareket Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Lokasyon gereklidir.")]
        [StringLength(150, ErrorMessage = "Lokasyon en fazla 150 karakter olabilir.")]
        [Display(Name = "Lokasyon")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama gereklidir.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kargo durumu gereklidir.")]
        [StringLength(80, ErrorMessage = "Kargo durumu en fazla 80 karakter olabilir.")]
        [Display(Name = "Kargo Durumu")]
        public string TrackingStatus { get; set; } = string.Empty;
    }
}
