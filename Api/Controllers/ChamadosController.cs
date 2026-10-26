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
    public async Task<IActionResult> Get()
    {
        var chamados = await _context.Chamados.ToListAsync();

        return Ok(chamados);
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