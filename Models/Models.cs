namespace BinanceApi.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    
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

}