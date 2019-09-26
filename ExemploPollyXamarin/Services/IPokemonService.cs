using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExemploPollyXamarin.Services
{
    public interface IPokemonService
    {
        Task<List<Pokemon>> GetPokemonsAsync();
    }
}
