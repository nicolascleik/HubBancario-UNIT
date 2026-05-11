using System;
using System.Threading;
using System.Threading.Tasks;
using BankingHub.Application.Commands.ProcessWebhook;
using BankingHub.Application.Interfaces;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankingHub.Application.Services
{
    public class PixReconciliationService : BackgroundService
    {
        private readonly IPixChargeRepository _pixChargeRepository;
        private readonly IBankPixAdapter _bankPixAdapter;
        private readonly IMediator _mediator;
        private readonly ILogger<PixReconciliationService> _logger;

        public PixReconciliationService(
            IPixChargeRepository pixChargeRepository,
            IBankPixAdapter bankPixAdapter,
            IMediator mediator,
            ILogger<PixReconciliationService> logger)
        {
            _pixChargeRepository = pixChargeRepository;
            _bankPixAdapter = bankPixAdapter;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
