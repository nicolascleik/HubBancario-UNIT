using AutoMapper;
using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Application.DTOs;

namespace HubBancario.Application.Mappings
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document.Value));

            CreateMap<PixKey, PixKeyDto>();
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Value));
                
            CreateMap<PixCharge, ChargeResponseDto>()
                .ForMember(dest => dest.QrCodeBase64, opt => opt.Ignore());
        }
    }
}