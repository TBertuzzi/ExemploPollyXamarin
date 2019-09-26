using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ExemploPollyXamarin.Services;
using HttpExtension;
using Newtonsoft.Json;
using Polly;

namespace ExemploPollyXamarin
{
    public class PokemonService : IPokemonService     {         public async Task<List<Pokemon>> GetPokemonsAsync()         {
            List<Pokemon> pokemons = new List<Pokemon>();

            try
            {
                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                string api = "https://pokeapi.co/api/v2/pokemon/";

                for (int i = 1; i < 20; i++)
                {

                    await Policy
                       .Handle<HttpRequestException>(ex => !ex.Message.ToLower().Contains("404"))
                       .WaitAndRetryAsync
                       (
                           retryCount: 3,
                           sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                           onRetry: (ex, time) =>
                           {
                               Console.WriteLine($"Ocorreu um erro ao baixar os dados: {ex.Message}, tentando novamente...");
                           }
                       )
                       .ExecuteAsync(async () =>
                       {
                           Console.WriteLine($"Obtendo pokemon...");

                           var resultJson = await httpClient.GetStringAsync($"{api}{i}");

                           return JsonConvert.DeserializeObject<IEnumerable<Pokemon>>(resultJson);
                       });


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return pokemons;         }     } 
}
