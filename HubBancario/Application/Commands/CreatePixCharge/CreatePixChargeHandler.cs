using System.Threading;
using System.Threading.Tasks;
using HubBancario.Application.DTOs;
using HubBancario.Application.Interfaces;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.Exceptions;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using MediatR;

namespace HubBancario.Application.Commands.CreatePixCharge
{
    public class CreatePixChargeHandler : IRequestHandler<CreatePixChargeCommand, QrCodeResponseDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBankAdapterFactory _bankAdapterFactory;
        private readonly IPixChargeRepository _pixChargeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePixChargeHandler(
            IInvoiceRepository invoiceRepository,
            IAccountRepository accountRepository,
            IBankAdapterFactory bankAdapterFactory,
            IPixChargeRepository pixChargeRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _accountRepository = accountRepository;
            _bankAdapterFactory = bankAdapterFactory;
            _pixChargeRepository = pixChargeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QrCodeResponseDto> Handle(CreatePixChargeCommand request, CancellationToken cancellationToken)
        {
            // 1. Busca os dados de origem
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
            if (invoice == null) throw new DomainException("Fatura não encontrada.");

            var account = await _accountRepository.GetByIdAsync(invoice.AccountId);

            // 2. O Hub gera o identificador oficial do Banco Central
            var txId = TxId.Generate("HUB");

            // 3. Descobre qual é o banco parceiro do cliente e pega o adaptador (ex: Itaú)
            var adapter = _bankAdapterFactory.GetAdapter(account.BankId);

            var chargeRequest = new ChargeRequestDto
            {
                TxId = txId.Value,
                Amount = invoice.Amount.Value,
                ChargeType = "COB",
                ExternalReference = invoice.ExternalReference
            };

            // 4. Chama a API do Banco de verdade
            var bankResponse = await adapter.GeneratePixAsync(chargeRequest);

            // 5. Salva a resposta como uma Cobrança Pix ativa no nosso banco
            var pixCharge = PixCharge.Create(txId, invoice.Id, PixChargeType.Cob, bankResponse.Emv, "{}");
            await _pixChargeRepository.AddAsync(pixCharge);
            
            await _unitOfWork.CommitAsync();

            // 6. Devolve o QRCode para o Frontend/ERP do lojista
            return new QrCodeResponseDto 
            { 
                TxId = txId.Value, 
                Emv = bankResponse.Emv, 
                QrCodeBase64 = bankResponse.QrCodeBase64 
            };
        }
    }
}