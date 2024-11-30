using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteKeeper.Aplicacao.ModuloCategoria;
using NoteKeeper.Dominio.ModuloCategoria;
using NoteKeeper.WebApi.ViewModels;

namespace NoteKeeper.WebApi.Controllers;

[Route("api/categorias")]
[ApiController]
public class CategoriaController(ServicoCategoria servicoCategoria, IMapper mapeador) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var resultado = await servicoCategoria.SelecionarTodosAsync();

        if (resultado.IsFailed)
            return StatusCode(500);

        var viewModel = mapeador.Map<ListarCategoriaViewModel[]>(resultado.Value);

        return Ok(viewModel);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var categoriaResult = await servicoCategoria.SelecionarPorIdAsync(id);

        if (categoriaResult.IsFailed)
            return StatusCode(500);

        if (categoriaResult.IsSuccess && categoriaResult.Value is null)
            return NotFound(categoriaResult.Errors);

        var viewModel = mapeador.Map<VisualizarCategoriaViewModel>(categoriaResult.Value);

        return Ok(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Post(InserirCategoriaViewModel categoriaVm)
    {
        var categoria = mapeador.Map<Categoria>(categoriaVm);

        var resultado = await servicoCategoria.InserirAsync(categoria);

        if (resultado.IsFailed)
            return BadRequest(resultado.Errors);

        return Ok(categoriaVm);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, EditarCategoriaViewModel categoriaVm)
    {
        var selecaoCategoriaOriginal = await servicoCategoria.SelecionarPorIdAsync(id);

        if (selecaoCategoriaOriginal.IsFailed)
            return NotFound(selecaoCategoriaOriginal.Errors);

        var categoriaEditada = mapeador.Map(categoriaVm, selecaoCategoriaOriginal.Value);

        var edicaoResult = await servicoCategoria.EditarAsync(categoriaEditada);

        if (edicaoResult.IsFailed)
            return BadRequest(edicaoResult.Errors);

        return Ok(edicaoResult.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var categoriaResult = await servicoCategoria.ExcluirAsync(id);

        if (categoriaResult.IsFailed)
            return NotFound(categoriaResult.Errors);

        return Ok();
    }
}