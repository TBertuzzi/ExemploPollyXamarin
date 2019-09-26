using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ExemploPollyXamarin.Services;
using HttpExtension;
using Newtonsoft.Json;
using Polly;

namespace ExemploPollyXamarin
{
    public class PokemonService : IPokemonService     {         readonly INetworkService _networkService;

        public PokemonService()
        {
            _networkService = new NetworkService();
        }

        async Task<Pokemon> PokemonGetRequest(string api, int id)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{api}{id}");

            var rawResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Pokemon>(rawResponse);
        }

        Task OnRetry(Exception e, int retryCount)
        {
            return Task.Factory.StartNew(() => {
                System.Diagnostics.Debug.WriteLine($"Tentativa #{retryCount}");
            });
        }


        public async Task<List<Pokemon>> GetPokemonsAsync()         {
            List<Pokemon> pokemons = new List<Pokemon>();

            try
            {
                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string api = "https://pokeapi.co/api/v2/pokemon/";

                for (int i = 1; i < 20; i++)
                {

                    // Forma simplificada
                    var func = new Func<Task<Pokemon>>(() => PokemonGetRequest(api, i));
                    pokemons.Add(await _networkService.Retry<Pokemon>(func, 3, OnRetry));



                    //Retry

                    //await Policy
                    //   .Handle<HttpRequestException>(ex => !ex.Message.ToLower().Contains("404"))
                    //   .WaitAndRetryAsync
                    //   (
                    //       retryCount: 3,
                    //       sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    //       onRetry: (ex, time) =>
                    //       {
                    //           Console.WriteLine($"Ocorreu um erro ao baixar os dados: {ex.Message}, tentando novamente...");
                    //       }
                    //   )
                    //   .ExecuteAsync(async () =>
                    //   {
                    //       Console.WriteLine($"Obtendo pokemon...");

                    //       var resultJson = await httpClient.GetStringAsync($"{api}{i}");

                    //       pokemons.Add(JsonConvert.DeserializeObject<Pokemon>(resultJson));
                    //   });


                    // Timeout
                    //var timeoutPolicy = Policy.TimeoutAsync(30);
                    //HttpResponseMessage httpResponse = await timeoutPolicy
                    //    .ExecuteAsync(
                    //      async ct => await httpClient.GetAsync($"{api}{i}", ct),
                    //      CancellationToken.None
                    //      );

                    //var resultJson = await httpResponse.Content.ReadAsStringAsync();

                    //pokemons.Add(JsonConvert.DeserializeObject<Pokemon>(resultJson));


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return pokemons;         }     } 
}
