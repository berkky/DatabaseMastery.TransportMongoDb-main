using System.ComponentModel.DataAnnotations;

namespace DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos
{
    public class CreateShipmentDto
    {
        [Required(ErrorMessage = "Takip numarası gereklidir.")]
        [StringLength(64, ErrorMessage = "Takip numarası en fazla 64 karakter olabilir.")]
        [Display(Name = "Takip Numarası")]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gönderici adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Gönderici adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Gönderici Adı")]
        public string SenderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gönderici telefonu gereklidir.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
        [StringLength(32, ErrorMessage = "Gönderici telefonu en fazla 32 karakter olabilir.")]
        [Display(Name = "Gönderici Telefonu")]
        public string SenderPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alıcı adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Alıcı adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Alıcı Adı")]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alıcı telefonu gereklidir.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
        [StringLength(32, ErrorMessage = "Alıcı telefonu en fazla 32 karakter olabilir.")]
        [Display(Name = "Alıcı Telefonu")]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çıkış ili gereklidir.")]
        [StringLength(100, ErrorMessage = "Çıkış ili en fazla 100 karakter olabilir.")]
        [Display(Name = "Çıkış İli")]
        public string DepartureCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çıkış ilçesi gereklidir.")]
        [StringLength(100, ErrorMessage = "Çıkış ilçesi en fazla 100 karakter olabilir.")]
        [Display(Name = "Çıkış İlçesi")]
        public string DepartureDistrict { get; set; } = string.Empty;

        [Required(ErrorMessage = "Varış ili gereklidir.")]
        [StringLength(100, ErrorMessage = "Varış ili en fazla 100 karakter olabilir.")]
        [Display(Name = "Varış İli")]
        public string ArrivalCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Varış ilçesi gereklidir.")]
        [StringLength(100, ErrorMessage = "Varış ilçesi en fazla 100 karakter olabilir.")]
        [Display(Name = "Varış İlçesi")]
        public string ArrivalDistrict { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres gereklidir.")]
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        [Display(Name = "Açık Adres")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Oluşturma Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Güncel durum gereklidir.")]
        [StringLength(80, ErrorMessage = "Güncel durum en fazla 80 karakter olabilir.")]
        [Display(Name = "Güncel Durum")]
        public string CurrentStatus { get; set; } = string.Empty;
    }
}
