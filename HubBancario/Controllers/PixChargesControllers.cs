using Microsoft.AspNetCore.Mvc;

namespace HubBancario.Controllers;

[ApiController]
[Route("api/pix-charges")]
public class PixChargesController : ControllerBase
{
    [HttpPost]
    public IActionResult CriarCobranca([FromBody] CriarCobrancaPixRequest request)
    {
        return Ok(new
        {
            mensagem = "Cobrança Pix recebida com sucesso.",
            dados = request
        });
    }

    [HttpGet("{txId}")]
    public IActionResult ConsultarStatus(string txId)
    {
        return Ok(new
        {
            txId = txId,
            status = "ATIVA"
        });
    }
}

public record CriarCobrancaPixRequest(
    Guid InvoiceId,
    string ChargeType,
    decimal Amount,
    string PixKey,
    DateOnly? DueDate,
    string? PayerMessage
);
