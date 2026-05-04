using Microsoft.AspNetCore.Mvc;

namespace HubBancario.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    [HttpPost]
    public IActionResult CriarFatura([FromBody] CriarFaturaRequest request)
    {
        return Ok(new
        {
            mensagem = "Fatura recebida com sucesso.",
            dados = request
        });
    }

    [HttpGet("{id}")]
    public IActionResult BuscarFatura(Guid id)
    {
        return Ok(new
        {
            id = id,
            status = "OPEN"
        });
    }
}

public record CriarFaturaRequest(
    decimal Amount,
    DateOnly DueDate,
    string BankId
);
