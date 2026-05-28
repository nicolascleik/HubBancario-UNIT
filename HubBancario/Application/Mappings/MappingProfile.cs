using AutoMapper;
using HubBancario.Application.Queries.GetInvoice;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Aggregates.Invoice;

namespace HubBancario.Application.Behaviors
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
