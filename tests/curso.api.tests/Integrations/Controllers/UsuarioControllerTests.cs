using curso.web.mvc;
using curso.web.mvc.Models.Usuarios;
using curso.api.Models.Usuarios;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using AutoBogus;
using curso.api.tests.Configuration;

namespace curso.api.tests.Integrations.Controllers
{
    public class UsuarioControllerTests : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Startup> _factory;
        protected readonly ITestOutputHelper _output;
        protected readonly HttpClient _httpClient;
        protected RegistroViewModelInput SignupViewModelInput;
        protected Models.Usuarios.LoginViewModelOutput LoginViewModelOutput;

        public async Task InitializeAsync()
        {
            await Registrar_InformandoUsuarioESenhaExistentes_DeveRetornarSucesso();
            await Logar_InformandoUsuarioESenhaExistentes_DeveRetornarSucesso();
        }

        public async Task DisposeAsync()
        {
            this._httpClient.Dispose();
        }

        public UsuarioControllerTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            this._factory = factory;
            this._output = output;
            this._httpClient = this._factory.CreateClient();
        }

        [Fact]
        public async Task Registrar_InformandoUsuarioESenhaExistentes_DeveRetornarSucesso()
        {
            // Arrange
            SignupViewModelInput = new AutoFaker<curso.api.Models.Usuarios.RegistroViewModelInput>(AutoBogusConfiguration.LOCATE)
                .RuleFor(p => p.Email, faker => faker.Person.Email)
                .RuleFor(l => l.Login, faker => faker.Person.FirstName + "." + faker.Person.LastName);
            StringContent content = new StringContent(JsonConvert.SerializeObject(SignupViewModelInput), Encoding.UTF8, "application/json");

            // Act
            var httpClientRequest = await this._httpClient.PostAsync("api/v1/usuario/registrar", content);

            // Assert
            _output.WriteLine($"{nameof(UsuarioControllerTests)}_{nameof(Registrar_InformandoUsuarioESenhaExistentes_DeveRetornarSucesso)} = {await httpClientRequest.Content.ReadAsStringAsync()}");
            Assert.Equal(HttpStatusCode.Created, httpClientRequest.StatusCode);
        }

        [Fact]
        public async Task Logar_InformandoUsuarioESenhaExistentes_DeveRetornarSucesso()
        {
            // Arrange
            var loginViewModelInput = new Models.Usuarios.LoginViewModelInput()
            {
                Login = SignupViewModelInput.Login,
                Senha = SignupViewModelInput.Senha
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(loginViewModelInput), Encoding.UTF8, "application/json");

            // Act
            var httpClientRequest = await this._httpClient.PostAsync("api/v1/usuario/logar", content);
            LoginViewModelOutput = JsonConvert.DeserializeObject<Models.Usuarios.LoginViewModelOutput>(await httpClientRequest.Content.ReadAsStringAsync());

            // Assert
            _output.WriteLine($"{nameof(UsuarioControllerTests)}_{nameof(Logar_InformandoUsuarioESenhaExistentes_DeveRetornarSucesso)} = {await httpClientRequest.Content.ReadAsStringAsync()} : {LoginViewModelOutput}");
            Assert.Equal(HttpStatusCode.OK, httpClientRequest.StatusCode);
            Assert.NotNull(LoginViewModelOutput.Token);
            Assert.Equal(loginViewModelInput.Login, LoginViewModelOutput.Usuario.Login);
        }
    }
}
