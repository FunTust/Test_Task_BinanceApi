using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Newtonsoft.Json;
using BinanceApi.Models;
using BinanceApi.Env;


namespace BinanceApi
{
    class BinanceApi
    {
        static HttpClient ApiClient = new HttpClient();
        public static void Main(string[] args)
        {
            StringBuilder path = new StringBuilder(Directory.GetCurrentDirectory());
            path.Append(@"\appsettings.env");
            DotEnv.Load(path.ToString());
            ApiClient.BaseAddress = new Uri("https://api.binance.com/api/");
            Thread Start = new Thread(o => Run().GetAwaiter());
            Start.Start();
            Timer t = new Timer(o => ExchangeInfoPrint().GetAwaiter(), null, 0, 60000);
            Console.ReadLine();
        }

        static async Task Run()
        {
            APIBase();
            await UserDataPrint();
        }


        static void ShowAPIInfo(API_rateLimits APIInfo)
        {
            if (APIInfo is null)
                {
                    throw new ArgumentNullException(nameof(APIInfo));
                }
                else
                {
                    foreach (var d in APIInfo.RateLimits)
                    {
                        Console.WriteLine("{0, -20}{2} per {1,-15}", d.RateLimitType, d.Interval, d.Limit);
                    }
                }
            
        }

        static void ShowtickerPrice(API_TickerPrice TickerPrice)
        {
            if (TickerPrice is null)
                {
                    throw new ArgumentNullException(nameof(TickerPrice));
                }
                else
                {
                    Console.WriteLine("*****************\n{0, -10}{1,-15}\n*****************", TickerPrice.symbol, TickerPrice.price);
                }
        }

        static void ShowUserData(API_UserData_model APIUserData)
        {
            if (APIUserData is null)
                {
                    throw new ArgumentNullException(nameof(APIUserData));
                }
                else
                {
                    foreach (var d in APIUserData.balances)
                    {
                        if(d.free!=0){
                            Console.WriteLine("{0, -10}{1,-15}\t{2,-15}\t", d.asset, d.free, d.locked);
                        }
                    }
                }
        }
        static async Task ExchangeInfoPrint()
        {
            Console.WriteLine(DateTime.Now);
            try
            { 
                ShowAPIInfo(await GetExchangeInfo());
                ShowtickerPrice(await GetTickerPrice());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static async Task UserDataPrint()
        {
            try
            { 
                ShowUserData(await GetAccountInfo());
                APIBase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task<API_rateLimits> GetExchangeInfo()
        {
            StringBuilder builder = new StringBuilder(ApiClient.BaseAddress.AbsoluteUri);
            builder.Append("v3/exchangeInfo");
            HttpResponseMessage response = await ApiClient.GetAsync(builder.ToString());
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<API_rateLimits>(jsonReader);
            }
            else
            {
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }

        static async Task<API_TickerPrice> GetTickerPrice()
        {
            StringBuilder builder = new StringBuilder(ApiClient.BaseAddress.AbsoluteUri);
            builder.Append("v3/ticker/price?symbol=BTCUSDT");
            HttpResponseMessage response = await ApiClient.GetAsync(builder.ToString());
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<API_TickerPrice>(jsonReader);
            }
            else
            {
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }

        static void APIBase()
        {
            ApiClient.DefaultRequestHeaders.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApiClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", Environment.GetEnvironmentVariable("API_KEY"));
        }

        static long GetTimestamp()
            {
                return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            }
        static async Task<API_UserData_model> GetAccountInfo()
        {
            StringBuilder url = new StringBuilder(ApiClient.BaseAddress.AbsoluteUri);
            url.Append("v3/account");
            StringBuilder args = new StringBuilder();
            args.Append("&timestamp=");
            args.Append(GetTimestamp());
            var asignature = CreateSignature(args.ToString());
            url.Append("?");
            url.Append(args);
            url.Append("&signature=");
            url.Append(asignature);
            HttpResponseMessage response = await ApiClient.GetAsync(url.ToString());
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<API_UserData_model>(jsonReader);
            }
            else
            {
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }

        static string CreateSignature(string queryString)
        {

            byte[] keyBytes = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY"));
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);
            
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}

