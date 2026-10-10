using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChamadosController : ControllerBase
{
    private static readonly List<Chamado> Chamados =
    [
        new Chamado
        {
            Id = 1,
            Titulo = "Impressora sem conexão",
            Descricao = "A impressora do setor não imprime.",
            Status = "Aberto"
        }
    ];

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Chamados);
    }

    [HttpPost]
    public IActionResult Post(Chamado chamado)
    {
        chamado.Id = Chamados.Max(c => c.Id) + 1;
        chamado.DataCriacao = DateTime.Now;

        Chamados.Add(chamado);

        return CreatedAtAction(nameof(Get), new { id = chamado.Id }, chamado);
    }
}
