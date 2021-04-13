#region

using System.Linq;
using System.Threading.Tasks;
using kpmg.Application.Dtos.UsuarioSistemaDtos;
using kpmg.Domain.Models;
using kpmg.Infrastructure.DataAccess;
using kpmg.Infrastructure.Repositories;
using kpmg.IntegrationTests.Helpers;
using kpmg.UnitTests.Tests.UsuarioSistemaTests.Bases;
using kpmg.WebApi.UseCases.V1.UsuarioSistemaApi;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace kpmg.IntegrationTests.Tests.UsuarioSistemaIntegrationTests
{
    public sealed class UsuarioSistemaControllerTests
    {
        private readonly ITestOutputHelper _output;

        private readonly UsuarioSistemaInjectionAppService _usuarioSistemaInjectionAppService = new();

        public UsuarioSistemaControllerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private UsuarioSistemaController ObterUsuarioSistemaController(KpmgContext context)
        {
            var mapper = MapperHelper.ConfigMapper();
            var usuarioSistemaAppService =
                _usuarioSistemaInjectionAppService.ObterUsuarioSistemaAppService(context, mapper);

            return new UsuarioSistemaController(usuarioSistemaAppService, mapper);
        }


        [Fact]
        public async Task Incluir_UsuarioSistema()
        {
            var options = new DbContextOptionsBuilder<KpmgContext>()
                .UseInMemoryDatabase("test_database_memoria_incluir_usuario_sistema")
                .Options;


            var teste = new UsuarioSistemaIncluirDto
            {
                Id = 1,
                Nome = "111",
                Email = "777@teste",
                Senha = "123456",
                Situacao = true,
                Matricula = "123"
            };


            await using var context = new KpmgContext(options);
            await context.Database.EnsureCreatedAsync();
            var usuarioSistemaController = ObterUsuarioSistemaController(context);
            _ = await usuarioSistemaController.Incluir(teste);
            Assert.Equal(1, context.UsuarioSistemas.Count());
        }

        [Fact]
        public async Task Obter_UsuarioSistema()
        {
            var options = new DbContextOptionsBuilder<KpmgContext>()
                .UseInMemoryDatabase("test_database_memoria_obter_usuario_sistema")
                .Options;

            UsuarioSistema usuarioSistema = null;

            await using var context = new KpmgContext(options);
            await context.Database.EnsureCreatedAsync();
            Utilities.InitializeDbForTests(context);
            var repository = new UsuarioSistemaRepository(context);
            usuarioSistema = await repository.GetById(1);
            Assert.NotNull(usuarioSistema);
        }
    }
}