using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Security.Cryptography;


namespace Test_Task
{
    public class API_filter_Model
    {
        [JsonPropertyName("filterType")]
        public string filterType;
    }
    public class API_BTCData_Model
    {
        [JsonPropertyName("quoteAsset")]
        public string quoteAsset;
    }
    public class API_balance_model
    {
        [JsonPropertyName("asset")]
        public string asset{ get; set; }
        [JsonPropertyName("free")]
        public float free{ get; set; }
        [JsonPropertyName("locked")]
        public float locked{ get; set; }
    }
    public class API_UserData_model
    {
        [JsonPropertyName("makerCommission")]
        public int makerCommission{ get; set; }
        [JsonPropertyName("takerCommission")]
        public int takerCommission{ get; set; }
        [JsonPropertyName("buyerCommission")]
        public int buyerCommission{ get; set; }
        [JsonPropertyName("sellerCommission")]
        public int sellerCommission{ get; set; }
        [JsonPropertyName("canTrade")]
        public bool canTrade{ get; set; }
        [JsonPropertyName("canWithdraw")]
        public bool canWithdraw{ get; set; }
        [JsonPropertyName("canDeposit")]
        public bool canDeposit{ get; set; }
        [JsonPropertyName("updateTime")]
        public long updateTime{ get; set; }
        [JsonPropertyName("accountType")]
        public string accountType{ get; set; }
        [JsonPropertyName("balances")]
        public List<API_balance_model> balances{ get; set; }
    }
    public class API_Info_model
    {
        [JsonPropertyName("rateLimitType")]
        public string RateLimitType { get; set; }
        [JsonPropertyName("interval")]
        public string Interval { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }

    public class API_rateLimits
    {  
        [JsonPropertyName("rateLimits")]
        public List<API_Info_model> RateLimits{ get; set; }
    }

    public class API_TickerPrice
    {
        [JsonPropertyName("symbol")]
        public string symbol;
        [JsonPropertyName("price")]
        public string price;
    }

    

    class BinanceApi
    {
        static string ApiKey { get; set; }
        static string SecretKey { get; set; }
        static HttpClient ApiClient = new HttpClient();
        static void Main()
        {
            ApiClient.BaseAddress = new Uri("https://api.binance.com");

            for(int i=1; i<4;i++){
                ChoseKey(i);
                Run();
            }
            int startin = DateTime.Now.Second;
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            var t = new System.Threading.Timer(o => ExchangeInfoPrint().GetAwaiter().GetResult(), null, 0, 60000);
            Console.ReadLine();
        }

        static void Run()
        {
            APIBase();
            UserDataPrint().GetAwaiter().GetResult();
        }
        static void ChoseKey(int UserNum)
        {
            Console.WriteLine("User: " + UserNum);
            if(UserNum == 1){
                ApiKey = "ApiKey";
                SecretKey = "SecretKey";
            }
            if(UserNum == 2){
                ApiKey = "ApiKey";
                SecretKey = "SecretKey";
            }
            if(UserNum == 3){
                ApiKey = "ApiKey";
                SecretKey = "SecretKey";
            }
        }

        static void ShowAPIInfo(API_rateLimits APIInfo)
        {
            foreach (var d in APIInfo.RateLimits)
                {
                    Console.WriteLine();
                    Console.WriteLine("{0, -20}{1,-15}\t{2,-15}\t", d.RateLimitType, d.Interval, d.Limit);
                }
        }

        static void ShowtickerPrice(API_TickerPrice TickerPrice)
        {
            Console.WriteLine("*****************");
            Console.WriteLine("{0, -10}{1,-15}", TickerPrice.symbol, TickerPrice.price);
            Console.WriteLine("*****************");
        }

        static void ShowUserData(API_UserData_model APIUserData)
        {
            foreach (var d in APIUserData.balances)
                {
                    Console.WriteLine("{0, -10}{1,-15}\t{2,-15}\t", d.asset, d.free, d.locked);
                }
                Console.WriteLine(APIUserData.balances.Count);
        }
        static async Task ExchangeInfoPrint()
        {
            Console.WriteLine("------------------");
            Console.WriteLine(DateTime.Now);
            try
            { 
                API_rateLimits  rateLimits = await GetExchangeInfo();
                if (rateLimits is null)
                {
                    throw new ArgumentNullException(nameof(rateLimits));
                }
                else
                {
                    ShowAPIInfo(rateLimits);
                }
                API_TickerPrice tickerPrice = await GetTickerPrice();
                if (tickerPrice is null)
                {
                    throw new ArgumentNullException(nameof(tickerPrice));
                }
                else
                {
                    ShowtickerPrice(tickerPrice);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static async Task UserDataPrint()
        {
            Console.WriteLine("------------------");
            Console.WriteLine(DateTime.Now);
            try
            { 
                API_UserData_model  AccountInfo = await GetAccountInfo();
                if (AccountInfo is null)
                {
                    throw new ArgumentNullException(nameof(AccountInfo));
                }
                else
                {
                    ShowUserData(AccountInfo);
                }
                APIBase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static async Task<API_rateLimits> GetExchangeInfo()
        {
            Uri  ExchangeInfoURL = new Uri(ApiClient.BaseAddress, "api/v3/exchangeInfo");
            HttpResponseMessage response = await ApiClient.GetAsync(ExchangeInfoURL);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<API_rateLimits>();
            }
            else
            {
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }

        static async Task<API_TickerPrice> GetTickerPrice()
        {
            string args = "?symbol=BTCUSDT";
            Uri  ExchangeInfoURL = new Uri(ApiClient.BaseAddress, "api/v3/ticker/price");
            HttpResponseMessage response = await ApiClient.GetAsync(ExchangeInfoURL+args);
            if (response.IsSuccessStatusCode)
            {
                // return null;
                return await response.Content.ReadAsAsync<API_TickerPrice>();
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
            ApiClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", ApiKey);
        }

        static long GetTimestamp()
            {
                return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            }
        static async Task<API_UserData_model> GetAccountInfo()
        {
            long timeStamp = GetTimestamp();
            Uri AccountInfo = new Uri(ApiClient.BaseAddress, "api/v3/account");
            string args = null;
            string headers = ApiClient.DefaultRequestHeaders.ToString();
            string timestamp = GetTimestamp().ToString();
            args += "&timestamp=" + timestamp;
            var signature = CreateSignature(args);
            HttpResponseMessage response = await ApiClient.GetAsync(AccountInfo+$"?{args}&signature={signature}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<API_UserData_model>();
            }
            else
            {
                Console.WriteLine("{0, -10}{1,-15}", (int)response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }

        static string CreateSignature(string queryString)
        {

            byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);
            
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}

