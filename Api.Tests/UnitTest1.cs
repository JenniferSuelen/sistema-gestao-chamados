using System.Net;
using System.Net.Http.Json;

namespace Api.Tests;

public class ChamadosApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ChamadosApiTests(
        CustomWebApplicationFactory factory
    )
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ComDadosValidos_DeveRetornarCreated()
    {
        // Arrange
        var novoChamado = new
        {
            titulo = "Erro no sistema de testes",
            descricao = "O sistema apresentou um problema durante o teste automatizado.",
            status = "Aberto"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            novoChamado
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Post_ComTituloVazio_DeveRetornarBadRequest()
    {
        // Arrange
        var chamadoInvalido = new
        {
            titulo = "",
            descricao = "O computador não está inicializando corretamente.",
            status = "Aberto"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            chamadoInvalido
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Post_ComDescricaoCurta_DeveRetornarBadRequest()
    {
        // Arrange
        var chamadoInvalido = new
        {
            titulo = "Erro no computador",
            descricao = "Falhou",
            status = "Aberto"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            chamadoInvalido
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Post_ComStatusInvalido_DeveRetornarBadRequest()
    {
        // Arrange
        var chamadoInvalido = new
        {
            titulo = "Erro no computador",
            descricao = "O computador não está inicializando corretamente.",
            status = "Pendente"
        };

        // Act
        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            chamadoInvalido
        );

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }
    [Fact]
public async Task CrudCompleto_DeveCriarBuscarAtualizarEExcluirChamado()
{
    // Arrange: preparar um chamado válido
    var novoChamado = new
    {
        titulo = "Problema na impressora",
        descricao = "A impressora do setor administrativo não está funcionando.",
        status = "Aberto"
    };

    // Act: cadastrar o chamado
    var respostaPost = await _client.PostAsJsonAsync(
        "/api/Chamados",
        novoChamado
    );

    // Assert: confirmar o cadastro
    Assert.Equal(
        HttpStatusCode.Created,
        respostaPost.StatusCode
    );

    var chamadoCriado =
        await respostaPost.Content.ReadFromJsonAsync<ChamadoResposta>();

    Assert.NotNull(chamadoCriado);
    Assert.True(chamadoCriado.Id > 0);
    Assert.Equal("Aberto", chamadoCriado.Status);

    // Act: buscar o chamado criado
    var respostaGet = await _client.GetAsync(
        $"/api/Chamados/{chamadoCriado.Id}"
    );

    // Assert: confirmar que foi encontrado
    Assert.Equal(
        HttpStatusCode.OK,
        respostaGet.StatusCode
    );

    var chamadoEncontrado =
        await respostaGet.Content.ReadFromJsonAsync<ChamadoResposta>();

    Assert.NotNull(chamadoEncontrado);
    Assert.Equal(
        "Problema na impressora",
        chamadoEncontrado.Titulo
    );

    // Arrange: preparar a atualização
    var chamadoAtualizado = new
    {
        titulo = "Problema na impressora",
        descricao = "A solicitação foi encaminhada para a equipe técnica.",
        status = "Em Andamento"
    };

    // Act: atualizar o chamado
    var respostaPut = await _client.PutAsJsonAsync(
        $"/api/Chamados/{chamadoCriado.Id}",
        chamadoAtualizado
    );

    // Assert: confirmar a atualização
    Assert.Equal(
        HttpStatusCode.OK,
        respostaPut.StatusCode
    );

    var chamadoDepoisDaAtualizacao =
        await respostaPut.Content.ReadFromJsonAsync<ChamadoResposta>();

    Assert.NotNull(chamadoDepoisDaAtualizacao);
    Assert.Equal(
        "Em Andamento",
        chamadoDepoisDaAtualizacao.Status
    );

    // Act: excluir o chamado
    var respostaDelete = await _client.DeleteAsync(
        $"/api/Chamados/{chamadoCriado.Id}"
    );

    // Assert: confirmar a exclusão
    Assert.Equal(
        HttpStatusCode.NoContent,
        respostaDelete.StatusCode
    );

    // Act: tentar encontrar o chamado excluído
    var respostaAposExclusao = await _client.GetAsync(
        $"/api/Chamados/{chamadoCriado.Id}"
    );

    // Assert: confirmar que não existe mais
    Assert.Equal(
        HttpStatusCode.NotFound,
        respostaAposExclusao.StatusCode
    );
}
}
public class ChamadoResposta
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime DataCriacao { get; set; }
}