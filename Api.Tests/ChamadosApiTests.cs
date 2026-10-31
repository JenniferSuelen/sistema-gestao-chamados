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
        var novoChamado = new
        {
            titulo = "Erro no sistema de testes",
            descricao = "O sistema apresentou um problema durante o teste automatizado.",
            status = "Aberto"
        };

        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            novoChamado
        );

        Assert.Equal(
            HttpStatusCode.Created,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Post_ComTituloVazio_DeveRetornarBadRequest()
    {
        var chamadoInvalido = new
        {
            titulo = "",
            descricao = "O computador não está inicializando corretamente.",
            status = "Aberto"
        };

        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            chamadoInvalido
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Post_ComDescricaoCurta_DeveRetornarBadRequest()
    {
        var chamadoInvalido = new
        {
            titulo = "Erro no computador",
            descricao = "Falhou",
            status = "Aberto"
        };

        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            chamadoInvalido
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Post_ComStatusInvalido_DeveRetornarBadRequest()
    {
        var chamadoInvalido = new
        {
            titulo = "Erro no computador",
            descricao = "O computador não está inicializando corretamente.",
            status = "Pendente"
        };

        var resposta = await _client.PostAsJsonAsync(
            "/api/Chamados",
            chamadoInvalido
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task CrudCompleto_DeveCriarBuscarAtualizarEExcluirChamado()
    {
        var novoChamado = new
        {
            titulo = "Problema na impressora",
            descricao = "A impressora do setor administrativo não está funcionando.",
            status = "Aberto"
        };

        var respostaPost = await _client.PostAsJsonAsync(
            "/api/Chamados",
            novoChamado
        );

        Assert.Equal(
            HttpStatusCode.Created,
            respostaPost.StatusCode
        );

        var chamadoCriado =
            await respostaPost.Content.ReadFromJsonAsync<ChamadoResposta>();

        Assert.NotNull(chamadoCriado);
        Assert.True(chamadoCriado.Id > 0);
        Assert.Equal("Aberto", chamadoCriado.Status);

        var respostaGet = await _client.GetAsync(
            $"/api/Chamados/{chamadoCriado.Id}"
        );

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

        var chamadoAtualizado = new
        {
            titulo = "Problema na impressora",
            descricao = "A solicitação foi encaminhada para a equipe técnica.",
            status = "Em Andamento"
        };

        var respostaPut = await _client.PutAsJsonAsync(
            $"/api/Chamados/{chamadoCriado.Id}",
            chamadoAtualizado
        );

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

        var respostaDelete = await _client.DeleteAsync(
            $"/api/Chamados/{chamadoCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            respostaDelete.StatusCode
        );

        var respostaAposExclusao = await _client.GetAsync(
            $"/api/Chamados/{chamadoCriado.Id}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            respostaAposExclusao.StatusCode
        );
    }

    [Fact]
    public async Task Get_ComStatusValido_DeveRetornarOk()
    {
        var novoChamado = new
        {
            titulo = "Problema de conexão",
            descricao = "O computador não consegue acessar a rede interna.",
            status = "Aberto"
        };

        var respostaPost = await _client.PostAsJsonAsync(
            "/api/Chamados",
            novoChamado
        );

        Assert.Equal(
            HttpStatusCode.Created,
            respostaPost.StatusCode
        );

        var resposta = await _client.GetAsync(
            "/api/Chamados?status=aberto&pagina=1&tamanhoPagina=10"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode
        );

        var resultado =
            await resposta.Content.ReadFromJsonAsync<RespostaPaginada>();

        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.PaginaAtual);
        Assert.Equal(10, resultado.TamanhoPagina);
        Assert.True(resultado.TotalRegistros >= 1);
        Assert.True(resultado.TotalPaginas >= 1);
        Assert.NotEmpty(resultado.Dados);

        Assert.All(
            resultado.Dados,
            chamado => Assert.Equal("Aberto", chamado.Status)
        );
    }

    [Fact]
    public async Task Get_ComStatusInvalido_DeveRetornarBadRequest()
    {
        var resposta = await _client.GetAsync(
            "/api/Chamados?status=Pendente&pagina=1&tamanhoPagina=10"
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Get_ComPaginaZero_DeveRetornarBadRequest()
    {
        var resposta = await _client.GetAsync(
            "/api/Chamados?pagina=0&tamanhoPagina=10"
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
        );
    }

    [Fact]
    public async Task Get_ComTamanhoPaginaAcimaDoLimite_DeveRetornarBadRequest()
    {
        var resposta = await _client.GetAsync(
            "/api/Chamados?pagina=1&tamanhoPagina=101"
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode
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

public class RespostaPaginada
{
    public int PaginaAtual { get; set; }

    public int TamanhoPagina { get; set; }

    public int TotalRegistros { get; set; }

    public int TotalPaginas { get; set; }

    public List<ChamadoResposta> Dados { get; set; } = [];
}