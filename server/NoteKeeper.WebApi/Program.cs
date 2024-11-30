using Microsoft.EntityFrameworkCore;
using NoteKeeper.Aplicacao.ModuloCategoria;
using NoteKeeper.Aplicacao.ModuloNota;
using NoteKeeper.Dominio.Compartilhado;
using NoteKeeper.Dominio.ModuloCategoria;
using NoteKeeper.Dominio.ModuloNota;
using NoteKeeper.Infra.Orm.Compartilhado;
using NoteKeeper.Infra.Orm.ModuloCategoria;
using NoteKeeper.Infra.Orm.ModuloNota;
using NoteKeeper.WebApi.Config.Mapping;
using NoteKeeper.WebApi.Config.Mapping.Actions;

namespace NoteKeeper.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        // Configuração de Injeção de Dependência
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddDbContext<IContextoPersistencia, NoteKeeperDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(connectionString, dbOptions =>
            {
                dbOptions.EnableRetryOnFailure();
            });
        });

        builder.Services.AddScoped<IRepositorioCategoria, RepositorioCategoriaOrm>();
        builder.Services.AddScoped<ServicoCategoria>();

        builder.Services.AddScoped<IRepositorioNota, RepositorioNotaOrm>();
        builder.Services.AddScoped<ServicoNota>();

        builder.Services.AddScoped<ConfigurarCategoriaMappingAction>();

        builder.Services.AddAutoMapper(config =>
        {
            config.AddProfile<CategoriaProfile>();
            config.AddProfile<NotaProfile>();
        });

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        // Middlewares de execução da API
        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        // Migrações de Banco de dados
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IContextoPersistencia>();

            if (dbContext is NoteKeeperDbContext noteKeeperDbContext)
            {
                MigradorBancoDados.AtualizarBancoDados(noteKeeperDbContext);
            }
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}