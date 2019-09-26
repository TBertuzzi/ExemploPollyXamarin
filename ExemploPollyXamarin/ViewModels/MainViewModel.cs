using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ExemploPollyXamarin.Services;

namespace ExemploPollyXamarin.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Pokemon> Pokemons { get; }
        private IPokemonService _pokemonService;

        public MainViewModel()
        {
            Pokemons = new ObservableCollection<Pokemon>();
            _pokemonService = new PokemonService(); // Aqui deveria ter uma Injeção de Dependencia

            LoadAsync();
        }
        public override async Task LoadAsync()
        {
            Ocupado = true;
            try
            {

                var pokemonsAPI = await _pokemonService.GetPokemonsAsync();


                Pokemons.Clear();

                foreach (var pokemon in pokemonsAPI)
                {
                    pokemon.Image = GetImageStreamFromUrl(pokemon.Sprites.FrontDefault.AbsoluteUri);
                    Pokemons.Add(pokemon);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro", ex.Message);
            }
            finally
            {
                Ocupado = false;
            }

        }

        public static byte[] GetImageStreamFromUrl(string url)
        {
            try
            {
                using (var webClient = new HttpClient())
                {
                    var imageBytes = webClient.GetByteArrayAsync(url).Result;

                    return imageBytes;

                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return null;

            }
        }
    }
}
