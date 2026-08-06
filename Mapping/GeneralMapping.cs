using AutoMapper;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Dtos.SliderDto;
using DatabaseMastery.TransportMongoDb.Dtos.BrandDtos;
using DatabaseMastery.TransportMongoDb.Dtos.OfferDto;
using DatabaseMastery.TransportMongoDb.Dtos.AboutDto;
using DatabaseMastery.TransportMongoDb.Dtos.GetInTouchDto;
using DatabaseMastery.TransportMongoDb.Dtos.HowItWorksDto;
using DatabaseMastery.TransportMongoDb.Dtos.ProjectDtos;
using DatabaseMastery.TransportMongoDb.Dtos.ShipmentDtos;
using DatabaseMastery.TransportMongoDb.Dtos.ShipmentTrackingDtos;
using DatabaseMastery.TransportMongoDb.Dtos.TestimonialDtos;

namespace DatabaseMastery.TransportMongoDb.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Slider, ResultSliderDto>().ReverseMap();
            CreateMap<Slider, CreateSliderDto>().ReverseMap();
            CreateMap<Slider, UpdateSliderDto>().ReverseMap();
            CreateMap<Slider, GetSliderByIdDto>().ReverseMap();

            CreateMap<Brand, ResultBrandDto>().ReverseMap();
            CreateMap<Brand, CreateBrandDto>().ReverseMap();
            CreateMap<Brand, UpdateBrandDto>().ReverseMap();
            CreateMap<Brand, GetBrandIdDto>().ReverseMap(); 


            CreateMap<Offer, GetOfferByIdDto>().ReverseMap();
            CreateMap<Offer, ResultOfferDto>().ReverseMap();
            CreateMap<Offer, CreateOfferDto>().ReverseMap();
            CreateMap<Offer, UpdateOfferDto>().ReverseMap();

            CreateMap<About, UpdateAboutDto>().ReverseMap();
            CreateMap<About, GetAboutByIdDto>().ReverseMap();
            CreateMap<About, ResultAboutDto>().ReverseMap();
            CreateMap<About, CreateAboutDto>().ReverseMap();

            CreateMap<GetInTouchSection, UpdateGetInTouchDto>().ReverseMap();
            CreateMap<GetInTouchSection, GetGetInTouchByIdDto>().ReverseMap();
            CreateMap<GetInTouchSection, ResultGetInTouchDto>().ReverseMap();
            CreateMap<GetInTouchSection, CreateGetInTouchDto>().ReverseMap();

            CreateMap<HowItWorks, UpdateHowItWorksDto>().ReverseMap();
            CreateMap<HowItWorks, GetHowItWorksByIdDto>().ReverseMap();
            CreateMap<HowItWorks, ResultHowItWorksDto>().ReverseMap();
            CreateMap<HowItWorks, CreateHowItWorksDto>().ReverseMap();

            CreateMap<Testimonial, UpdateTestimonialDto>().ReverseMap();
            CreateMap<Testimonial, GetTestimonialByIdDto>().ReverseMap();
            CreateMap<Testimonial, ResultTestimonialDto>().ReverseMap();
            CreateMap<Testimonial, CreateTestimonialDto>().ReverseMap();

            CreateMap<Project, UpdateProjectDto>().ReverseMap();
            CreateMap<Project, GetProjectByIdDto>().ReverseMap();
            CreateMap<Project, ResultProjectDto>().ReverseMap();
            CreateMap<Project, CreateProjectDto>().ReverseMap();

            CreateMap<Shipment, UpdateShipmentDto>().ReverseMap();
            CreateMap<Shipment, GetShipmentByIdDto>().ReverseMap();
            CreateMap<Shipment, ResultShipmentDto>().ReverseMap();
            CreateMap<Shipment, CreateShipmentDto>().ReverseMap();

            CreateMap<ShipmentTracking, CreateShipmentTrackingDto>().ReverseMap();
            CreateMap<ShipmentTracking, ResultShipmentTrackingDto>().ReverseMap();
            CreateMap<ShipmentTracking, UpdateShipmentTrackingDto>().ReverseMap();
        }
    }
}
