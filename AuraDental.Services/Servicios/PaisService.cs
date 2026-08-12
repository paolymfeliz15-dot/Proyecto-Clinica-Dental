using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using AuraDental.Aplicacion.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace AuraDental.Aplicacion
{
    public class PaisService : IPaisService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

        public PaisService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<string>> ObtenerPaisesAsync()
        {
            const string cacheKey = "localizacion_paises";
            if (_cache.TryGetValue(cacheKey, out List<string>? cacheado) && cacheado != null)
                return cacheado;

            var respaldo = new List<string> { "Dominican Republic", "United States", "Spain", "Mexico", "Colombia" };

            try
            {
                var json = await _httpClient.GetStringAsync("https://countriesnow.space/api/v0.1/countries/iso");
                var respuesta = JsonSerializer.Deserialize<PaisesRespuestaDto>(json, JsonOpts);

                var nombres = respuesta?.Data?
                    .Select(p => p.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .OrderBy(n => n)
                    .ToList();

                if (nombres == null || !nombres.Any()) return respaldo;

                _cache.Set(cacheKey, nombres, TimeSpan.FromHours(24));
                return nombres;
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
            {
                return respaldo;
            }
        }

        public async Task<List<string>> ObtenerEstadosAsync(string pais)
        {
            if (string.IsNullOrWhiteSpace(pais)) return new List<string>();

            var cacheKey = $"localizacion_estados_{pais}";
            if (_cache.TryGetValue(cacheKey, out List<string>? cacheado) && cacheado != null)
                return cacheado;

            try
            {
                var body = new StringContent(
                    JsonSerializer.Serialize(new { country = pais }),
                    Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://countriesnow.space/api/v0.1/countries/states", body);
                var json = await response.Content.ReadAsStringAsync();
                var respuesta = JsonSerializer.Deserialize<EstadosRespuestaDto>(json, JsonOpts);

                var nombres = respuesta?.Data?.States?
                    .Select(e => LimpiarNombreEstado(e.Name))
                    .OrderBy(n => n)
                    .ToList() ?? new List<string>();

                _cache.Set(cacheKey, nombres, TimeSpan.FromHours(24));
                return nombres;
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
            {
                return new List<string>();
            }
        }

        // La API de CountriesNow a veces devuelve el nombre con un sufijo genérico
        // (ej. "Barahona Province", "Texas State") que no aporta nada al usuario
        // y se ve poco estético — lo quitamos, dejando solo el nombre real del lugar.
        private static string LimpiarNombreEstado(string nombre)
        {
            var sufijos = new[] { " Province", " State", " Department", " Region", " District", " County", " Governorate", " Prefecture", " Territory" };

            foreach (var sufijo in sufijos)
            {
                if (nombre.EndsWith(sufijo, StringComparison.OrdinalIgnoreCase))
                    return nombre[..^sufijo.Length].Trim();
            }

            return nombre;
        }

        public async Task<List<string>> ObtenerCiudadesAsync(string pais, string estado)
        {
            if (string.IsNullOrWhiteSpace(pais) || string.IsNullOrWhiteSpace(estado)) return new List<string>();

            var cacheKey = $"localizacion_ciudades_{pais}_{estado}";
            if (_cache.TryGetValue(cacheKey, out List<string>? cacheado) && cacheado != null)
                return cacheado;

            try
            {
                var body = new StringContent(
                    JsonSerializer.Serialize(new { country = pais, state = estado }),
                    Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://countriesnow.space/api/v0.1/countries/state/cities", body);
                var json = await response.Content.ReadAsStringAsync();
                var respuesta = JsonSerializer.Deserialize<CiudadesRespuestaDto>(json, JsonOpts);

                var nombres = (respuesta?.Data ?? new List<string>()).OrderBy(n => n).ToList();

                _cache.Set(cacheKey, nombres, TimeSpan.FromHours(6));
                return nombres;
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
            {
                return new List<string>();
            }
        }
    }
}