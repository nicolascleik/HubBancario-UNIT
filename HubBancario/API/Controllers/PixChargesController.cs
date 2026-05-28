using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.CreatePixCharge;
using HubBancario.Application.Queries.GetPixChargeStatus;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Controllers
{
    [ApiController]
    [Route("api/pix-charges")]
    [Produces("application/json")]
    public class PixChargesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PixChargesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Emite uma nova cobrança Pix dinâmica (EMV/Cópia e Cola e Base64 do QR Code) para uma fatura existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint dispara o fluxo completo da Clean Architecture: localiza a fatura,
        /// aciona o BankAdapterFactory para obter o adaptador correspondente (ex: Itaú),
        /// gera o TxId oficial, faz o registro em Open Banking e salva a cobrança ativa na base.
        /// </remarks>
        /// <param name="command">Dados da cobrança contendo o ID da fatura (InvoiceId) e o tipo de cobrança (Cob ou CobV).</param>
        /// <returns>Retorna os dados do Pix gerado incluindo o TxId e o payload Pix Copia e Cola.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePixChargeCommand command)
        {
            var result = await _mediator.Send(command);

            // Retorna HTTP 201 Created apontando para a URL do recurso de consulta de status
            return CreatedAtAction(
                nameof(GetStatus), 
                new { txId = result.TxId }, 
                result);
        }

        /// <summary>
        /// Realiza a consulta ativa do status de uma cobrança Pix utilizando o identificador único TxId.
        /// </summary>
        /// <remarks>
        /// Este endpoint expõe a estratégia clássica de leitura do CQRS (Queries), 
        /// retornando de forma otimizada o estado atual registrado no banco de dados do Hub.
        /// </remarks>
        /// <param name="txId">O identificador de transação Pix (TxId) de até 35 caracteres alfanuméricos.</param>
        /// <returns>Retorna o status atual da cobrança (ex: ACTIVE, PAID, EXPIRED).</returns>
        [HttpGet("{txId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatus([FromRoute] string txId)
        {
            if (string.IsNullOrWhiteSpace(txId))
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Parâmetro Inválido",
                    Detail = "O TxId fornecido na rota não pode ser nulo ou vazio."
                });
            }

            var query = new GetPixChargeStatusQuery { TxId = txId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}