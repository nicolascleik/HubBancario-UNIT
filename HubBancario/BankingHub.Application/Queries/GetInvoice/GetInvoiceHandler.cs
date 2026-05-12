using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HubBancario.Domain.Repositories;
using MediatR;

namespace BankingHub.Application.Queries.GetInvoice
{
    public class GetInvoiceHandler : IRequestHandler<GetInvoiceQuery, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetInvoiceHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<InvoiceDto> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
