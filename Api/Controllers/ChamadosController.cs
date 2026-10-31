using Api.Data;
using Api.DTOs;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChamadosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChamadosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
public async Task<IActionResult> Get(
    [FromQuery] string? status,
    [FromQuery] int pagina = 1,
    [FromQuery] int tamanhoPagina = 10
)
{
    if (pagina < 1 || pagina > 100000)
    {
        return BadRequest(new
        {
            mensagem = "A página deve estar entre 1 e 100000."
        });
    }

    if (tamanhoPagina < 1 || tamanhoPagina > 100)
    {
        return BadRequest(new
        {
            mensagem = "O tamanho da página deve estar entre 1 e 100."
        });
    }

    var statusPermitidos = new[]
    {
        "Aberto",
        "Em Andamento",
        "Resolvido",
        "Fechado"
    };

    if (!string.IsNullOrWhiteSpace(status) &&
        !statusPermitidos.Contains(
            status,
            StringComparer.OrdinalIgnoreCase
        ))
    {
        return BadRequest(new
        {
            mensagem = "O status deve ser: Aberto, Em Andamento, Resolvido ou Fechado."
        });
    }

    var consulta = _context.Chamados
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(status))
    {
        consulta = consulta.Where(
            chamado => chamado.Status.ToLower() == status.ToLower()
        );
    }

    var totalRegistros = await consulta.CountAsync();

    var chamados = await consulta
        .OrderByDescending(chamado => chamado.DataCriacao)
        .ThenByDescending(chamado => chamado.Id)
        .Skip((pagina - 1) * tamanhoPagina)
        .Take(tamanhoPagina)
        .ToListAsync();

    var totalPaginas = (int)Math.Ceiling(
        totalRegistros / (double)tamanhoPagina
    );

    return Ok(new
    {
        paginaAtual = pagina,
        tamanhoPagina,
        totalRegistros,
        totalPaginas,
        dados = chamados
    });
}

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var chamado = await _context.Chamados.FindAsync(id);

        if (chamado == null)
        {
           return NotFound(new
{
    mensagem = $"Chamado com ID {id} não encontrado."
});
        }

        return Ok(chamado);
    }

    [HttpPost]
public async Task<IActionResult> Post(CriarChamadoDto dto)
{
    var chamado = new Chamado
    {
        Titulo = dto.Titulo.Trim(),
        Descricao = dto.Descricao.Trim(),
        Status = dto.Status,
        DataCriacao = DateTime.Now
    };

    _context.Chamados.Add(chamado);
    await _context.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetById),
        new { id = chamado.Id },
        chamado
    );
}

   [HttpPut("{id}")]
public async Task<IActionResult> Put(
    int id,
    AtualizarChamadoDto dto
)
{
    var chamado = await _context.Chamados.FindAsync(id);

    if (chamado == null)
    {
        return NotFound(new
        {
            mensagem = $"Chamado com ID {id} não encontrado."
        });
    }

    chamado.Titulo = dto.Titulo.Trim();
    chamado.Descricao = dto.Descricao.Trim();
    chamado.Status = dto.Status;

    await _context.SaveChangesAsync();

    return Ok(chamado);
}

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var chamado = await _context.Chamados.FindAsync(id);

        if (chamado == null)
        {
            return NotFound();
        }

        _context.Chamados.Remove(chamado);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}