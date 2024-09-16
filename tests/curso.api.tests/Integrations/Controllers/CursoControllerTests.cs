using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoBogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;
using System.Net.Http.Headers;
using curso.api.Models.Cursos;
using System.Collections;

namespace curso.api.tests.Integrations.Controllers
{
    public class CursoControllerTests : UsuarioControllerTests
    {
        public CursoControllerTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output) : base(factory, output)
        {

        }

        [Fact]
        public async Task Registrar_InformandoUsuarioAutenticadoEDadosDeUmNovoCursoValido_DeveRetornarSucesso()
        {
            // Arrange
            var cursoViewModelInputFaker = new AutoFaker<CursoViewModelInput>();
            var cursoViewModelInput = cursoViewModelInputFaker.Generate();
            StringContent content = new StringContent(JsonConvert.SerializeObject(cursoViewModelInput), Encoding.UTF8, "application/json");
            this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginViewModelOutput.Token);

            // Act
            var httpClientRequest = await this._httpClient.PostAsync("api/v1/cursos", content);

            // Assert
            _output.WriteLine($"{nameof(CursoControllerTests)}_{nameof(Registrar_InformandoUsuarioAutenticadoEDadosDeUmNovoCursoValido_DeveRetornarSucesso)} = {await httpClientRequest.Content.ReadAsStringAsync()}");
            Assert.Equal(HttpStatusCode.Created, httpClientRequest.StatusCode);
        }

        [Fact]
        public async Task Registrar_InformandoUsuarioNaoAutenticadoEDadosDeUmNovoCursoValido_DeveRetornarNaoAutorizado()
        {
            // Arrange
            var cursoViewModelInputFaker = new AutoFaker<CursoViewModelInput>();
            var cursoViewModelInput = cursoViewModelInputFaker.Generate();
            StringContent content = new StringContent(JsonConvert.SerializeObject(cursoViewModelInput), Encoding.UTF8, "application/json");

            // Act
            var httpClientRequest = await this._httpClient.PostAsync("api/v1/cursos", content);

            // Assert
            _output.WriteLine($"{nameof(CursoControllerTests)}_{nameof(Registrar_InformandoUsuarioNaoAutenticadoEDadosDeUmNovoCursoValido_DeveRetornarNaoAutorizado)} = {await httpClientRequest.Content.ReadAsStringAsync()}");
            Assert.Equal(HttpStatusCode.Unauthorized, httpClientRequest.StatusCode);
        }

        [Fact]
        public async Task Obter_InformandoUsuarioAutenticado_DeveRetornarSucesso()
        {
            // Arrange
            await Registrar_InformandoUsuarioAutenticadoEDadosDeUmNovoCursoValido_DeveRetornarSucesso();
            this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginViewModelOutput.Token);

            // Act
            var httpClientRequest = await this._httpClient.GetAsync("api/v1/cursos");
            var cursos = JsonConvert.DeserializeObject<IList<CursoViewModelOutput>>(await httpClientRequest.Content.ReadAsStringAsync());

            // Assert
            _output.WriteLine($"{nameof(CursoControllerTests)}_{nameof(Obter_InformandoUsuarioAutenticado_DeveRetornarSucesso)} = {await httpClientRequest.Content.ReadAsStringAsync()}");
            Assert.Equal(HttpStatusCode.OK, httpClientRequest.StatusCode);
            Assert.NotEmpty(cursos);
        }
    }
}
