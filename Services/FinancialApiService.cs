using FinancialApp.Data;
using FinancialApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinancialApp.Services
{
    public class FinancialApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FinancialApiService> _logger;
        private readonly AppDbContext _dbContext;
        private const string ApiKey = "eQfUnvjUhH2rtGelyAyrXPUg4M6DiRdb"; 
        


        public FinancialApiService(HttpClient httpClient, ILogger<FinancialApiService> logger, AppDbContext dbContext)
        {
            _httpClient = httpClient;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<List<Stock>?> GetStockHistory(string symbol)
        {
            var startDate = DateTime.UtcNow.AddMonths(-3).ToString("yyyy-MM-dd");  

            var endDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");             try
            {
                var existingStockData = await _dbContext.Stocks
                    .Where(s => s.Symbol == symbol && s.Date >= DateTime.Parse(startDate) && s.Date <= DateTime.Parse(endDate))
                    .ToListAsync();

                if (existingStockData.Any())
                {
                    return existingStockData;
                }

                string QUERY_URL = $"https://api.polygon.io/v2/aggs/ticker/{symbol}/range/1/day/{startDate}/{endDate}?apiKey={ApiKey}";
                var response = await _httpClient.GetAsync(QUERY_URL);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stockHistory = ParseStockHistory(jsonString, symbol);

                    if (stockHistory != null && stockHistory.Any())
                    {
                        await SaveStockHistory(stockHistory);
                    }

                    return stockHistory;
                }
                else
                {
                    _logger.LogError($"Failed to fetch stock history: {response.ReasonPhrase}");
                    throw new Exception($"Failed to fetch stock history: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return null;
            }
        }

        private async Task SaveStockHistory(List<Stock> stockHistory)
        {
            _dbContext.Stocks.AddRange(stockHistory);
            await _dbContext.SaveChangesAsync();
        }

        private List<Stock>? ParseStockHistory(string jsonString, string symbol)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("results", out var results))
                {
                    _logger.LogError("Results property not found in the JSON response.");
                    return null;
                }

                var stockHistory = new List<Stock>();

                foreach (JsonElement result in results.EnumerateArray())
                {
                    var stock = new Stock
                    {
                        Symbol = symbol,
                        Price = result.GetProperty("c").GetDouble(), 
                        Date = DateTimeOffset.FromUnixTimeMilliseconds(result.GetProperty("t").GetInt64()).DateTime 
                    };

                    stockHistory.Add(stock);
                }

                return stockHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in ParseStockHistory: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Crypto>?> GetCryptoWeekly(string symbol)
        {
            var startDate = DateTime.UtcNow.AddMonths(-3).ToString("yyyy-MM-dd");  

            var endDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");  
            try
            {
                var existingCryptoData = await _dbContext.Cryptos
                    .Where(c => c.Symbol == symbol && c.Date >= DateTime.Parse(startDate) && c.Date <= DateTime.Parse(endDate))
                    .ToListAsync();

                if (existingCryptoData.Any())
                {
                    return existingCryptoData;
                }

                string QUERY_URL = $"https://api.polygon.io/v2/aggs/ticker/{symbol}/range/1/day/{startDate}/{endDate}?apiKey={ApiKey}";
                var response = await _httpClient.GetAsync(QUERY_URL);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var cryptoHistory = ParseCryptoWeekly(jsonString, symbol);

                    if (cryptoHistory != null && cryptoHistory.Any())
                    {
                        await SaveCryptoWeekly(cryptoHistory);
                    }

                    return cryptoHistory;
                }
                else
                {
                    _logger.LogError($"Failed to fetch crypto weekly data: {response.ReasonPhrase}");
                    throw new Exception($"Failed to fetch crypto weekly data: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return null;
            }
        }

        private async Task SaveCryptoWeekly(List<Crypto> cryptoHistory)
        {
            _dbContext.Cryptos.AddRange(cryptoHistory);
            await _dbContext.SaveChangesAsync();
        }

        private List<Crypto>? ParseCryptoWeekly(string jsonString, string symbol)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("results", out var results))
                {
                    _logger.LogError("Results property not found in the JSON response.");
                    return null;
                }

                var cryptoHistory = new List<Crypto>();

                foreach (JsonElement result in results.EnumerateArray())
                {
                    var crypto = new Crypto
                    {
                        Symbol = symbol,
                        Price = result.GetProperty("c").GetDouble(), 
                        Date = DateTimeOffset.FromUnixTimeMilliseconds(result.GetProperty("t").GetInt64()).DateTime
                    };

                    cryptoHistory.Add(crypto);
                }

                return cryptoHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in ParseCryptoWeekly: {ex.Message}");
                return null;
            }
        }
    }
}
