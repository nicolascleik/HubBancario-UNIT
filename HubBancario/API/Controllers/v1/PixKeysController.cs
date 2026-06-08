using System;
using System.Threading.Tasks;
using HubBancario.Application.Commands.PixKey.CreatePixKey;
using HubBancario.Application.Commands.PixKey.DeletePixKey;
using HubBancario.Application.DTOs;
using HubBancario.Application.Queries.PixKey;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubBancario.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/pix-keys")]
    [Produces("application/json")]
    public class PixKeysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PixKeysController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Regista e vincula uma nova chave Pix a uma conta lojista ativa.
        /// </summary>
        /// <param name="command">Dados necessários para a criação da chave Pix.</param>
        /// <returns>O ID do registo da chave Pix criada no Hub.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreatePixKeyCommand command)
        {
            var id = await _mediator.Send(command);

            // Retorna o HTTP 201 Created com a estrutura padronizada
            return CreatedAtAction(nameof(Create), new { id }, new { Id = id });
        }

        /// <summary>
        /// Consulta informações de uma chave Pix.
        /// </summary>
        /// <param name="keyValue">Valor da chave Pix.</param>
        /// <returns>Informações da chave Pix.</returns>
        [HttpGet("{keyValue}")]
        [ProducesResponseType(typeof(PixKeyInfoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string keyValue)
        {
            if (string.IsNullOrWhiteSpace(keyValue))
                return BadRequest("A chave Pix deve ser informada.");

            var query = new GetPixKeyInfoQuery
            {
                KeyValue = keyValue
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Remove ou inativa uma chave Pix do sistema do Hub.
        /// </summary>
        /// <param name="id">Identificador único (Guid) da chave Pix no banco de dados.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeletePixKeyCommand { Id = id };

            await _mediator.Send(command);

            // Retorno HTTP 204 (No Content) padrão REST para deleções com sucesso
            return NoContent();
        }
    }
}
