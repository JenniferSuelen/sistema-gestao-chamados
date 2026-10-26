using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class CriarChamadoDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(
        100,
        MinimumLength = 3,
        ErrorMessage = "O título deve ter entre 3 e 100 caracteres."
    )]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(
        1000,
        MinimumLength = 10,
        ErrorMessage = "A descrição deve ter entre 10 e 1000 caracteres."
    )]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O status é obrigatório.")]
    [RegularExpression(
        "^(Aberto|Em Andamento|Resolvido|Fechado)$",
        ErrorMessage = "O status deve ser: Aberto, Em Andamento, Resolvido ou Fechado."
    )]
    public string Status { get; set; } = "Aberto";
}
