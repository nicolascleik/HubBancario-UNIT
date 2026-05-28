using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HubBancario.Application.DTOs;
using HubBancario.Domain.Repositories;
using MediatR;

namespace HubBancario.Application.Queries.GetInvoice
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
            // Busca o agregado puro do domínio assincronamente
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);

            if (invoice == null)
                return null;

            // Transforma o agregado rico em um DTO simples e plano para a API externa
            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}