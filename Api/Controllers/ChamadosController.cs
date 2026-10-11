using Api.Data;
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
            return NotFound();
        }

        return Ok(chamado);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Chamado chamado)
    {
        chamado.Id = 0;
        chamado.DataCriacao = DateTime.Now;

        _context.Chamados.Add(chamado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = chamado.Id },
            chamado
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Chamado chamadoAtualizado)
    {
        var chamado = await _context.Chamados.FindAsync(id);

        if (chamado == null)
        {
            return NotFound();
        }

        chamado.Titulo = chamadoAtualizado.Titulo;
        chamado.Descricao = chamadoAtualizado.Descricao;
        chamado.Status = chamadoAtualizado.Status;

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