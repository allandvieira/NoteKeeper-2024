using AutoMapper;
using NoteKeeper.Dominio.ModuloCategoria;
using NoteKeeper.WebApi.ViewModels;

namespace NoteKeeper.WebApi.Config.Mapping;

public class CategoriaProfile : Profile
{
    public CategoriaProfile()
    {
        CreateMap<Categoria, ListarCategoriaViewModel>();

        CreateMap<InserirCategoriaViewModel, Categoria>();
        CreateMap<EditarCategoriaViewModel, Categoria>();
    }
}
