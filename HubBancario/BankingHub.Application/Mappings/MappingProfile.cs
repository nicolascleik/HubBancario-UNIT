using AutoMapper;
using BankingHub.Application.Queries.GetInvoice;
using HubBancario.Domain.Aggregates.Invoice;

namespace BankingHub.Application.Mappings
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
