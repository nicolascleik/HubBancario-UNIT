using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.CreateInvoice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/invoices")]
    [Produces("application/json")]
    public class InvoicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Emite uma nova fatura (Invoice) de cobrança para um cliente do lojista.
        /// </summary>
        /// <remarks>
        /// Esta operação registra a intenção de faturamento no sistema. 
        /// Após a criação da fatura, o ecossistema estará pronto para a emissão da respectiva cobrança Pix.
        /// </remarks>
        /// <param name="command">Dados do faturamento, incluindo valor (Amount), data de vencimento (DueDate) e referências externas.</param>
        /// <returns>O identificador único (Guid) da fatura gerada no Hub.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command)
        {
            // Despacha o comando de criação da fatura diretamente para o MediatR
            var id = await _mediator.Send(command);

            // Retorna o HTTP 201 Created indicando o ID do recurso gerado
            return CreatedAtAction(nameof(Create), new { id }, new { Id = id });
        }
    }
}