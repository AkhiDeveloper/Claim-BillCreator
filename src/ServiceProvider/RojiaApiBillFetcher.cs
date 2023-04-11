using src.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http.Headers;

namespace src.ServiceProvider
{
    internal class RojiaApiBillFetcher
        : IBillFetcher
    {
        private readonly HttpClient _httpClient;
        private string _authToken;

        public RojiaApiBillFetcher(string base_address, string username, string password)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(base_address);
            this.Login(username, password).Wait();
        }

        private async Task Login(string username, string password)
        {
            string path = "Assets/Json/LoginRequestBody.json";
            string requestBodyJson = File.ReadAllText(path);
            JObject reqObj = JObject.Parse(requestBodyJson);
            reqObj["variables"]["username"] = username;
            reqObj["variables"]["password"] = password;

            //Sending Request
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StringContent(reqObj.ToString(), encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            JObject responseObj = JObject.Parse(await response.Content.ReadAsStringAsync());
            _authToken = responseObj["data"]["login"]["token"].ToString();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _authToken);
        }

        private async Task<Tab> FetchTab(DateOnly from, DateOnly to, string? userId)
        {
            string path = "Assets/Json/TabRequestBody.json";
            string requestBodyJson = File.ReadAllText(path);
            JObject reqObj = JObject.Parse(requestBodyJson);
            reqObj["variables"]["filter"]["dateRange"]["start"] = from.ToString("yyyy-MM-dd");
            reqObj["variables"]["filter"]["dateRange"]["end"] = to.ToString("yyyy-MM-dd");
            if(userId != null)
            {
                reqObj["variables"]["filter"]["filters"][0]["column"] = "user_id";
                reqObj["variables"]["filter"]["filters"][0]["value"][0] = userId;
            }

            //Sending Request
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StringContent(reqObj.ToString(), encoding: Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue(_authToken);
            var response = await _httpClient.SendAsync(request);
            JObject responseObj = JObject.Parse(await response.Content.ReadAsStringAsync());
            var result = new Tab()
            {
                Recieved = int.Parse(responseObj["data"]["allOrderCount"]["received"].ToString()),
                Invoiced = int.Parse(responseObj["data"]["allOrderCount"]["invoiced"].ToString()),
                Dispatched = int.Parse(responseObj["data"]["allOrderCount"]["dispatched"].ToString()),
                Delivered = int.Parse(responseObj["data"]["allOrderCount"]["delivered"].ToString()),
                Replaced = int.Parse(responseObj["data"]["allOrderCount"]["replaced"].ToString()),
            };
            return result;
        }

        public async Task<IEnumerable<Bill>> FetchInvoices(DateOnly from, DateOnly to, string? userId)
        {
            IList<Bill> result = new List<Bill>();
            var tab = await this.FetchTab(from, to, userId);
            string path = "Assets/Json/InvoicedRequestBody.json";
            string requestBodyJson = File.ReadAllText(path);
            JObject reqObj = JObject.Parse(requestBodyJson);
            reqObj["variables"]["filter"]["dateRange"]["start"] = from.ToString("yyyy-MM-dd");
            reqObj["variables"]["filter"]["dateRange"]["end"] = to.ToString("yyyy-MM-dd");
            if (userId != null)
            {
                reqObj["variables"]["filter"]["filters"][0]["column"] = "user_id";
                reqObj["variables"]["filter"]["filters"][0]["value"][0] = userId;
            }
            reqObj["variables"]["offset"] = 0;
            reqObj["variables"]["limit"] = tab.Invoiced;

            //Sending Request
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StringContent(reqObj.ToString(), encoding: Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue(_authToken);
            var response = await _httpClient.SendAsync(request);
            JObject responseObj = JObject.Parse(await response.Content.ReadAsStringAsync());

            JArray BillsObj = (JArray)responseObj["data"]["invoices"]["rows"];
            foreach(var billObj in BillsObj)
            {
                Bill bill = new Bill();
                bill.OutletId = billObj["RetailOutlet"]["id"].ToString();
                bill.OutletName = billObj["RetailOutlet"]["title"].ToString();
                int billId = int.Parse(billObj["id"].ToString());
                bill.Items = (await this.FetchOrders(billId)).ToList();
                result.Add(bill);
            }
            return result;
        }

        private async Task<IEnumerable<BillItem>> FetchOrders(int id)
        {
            IList<BillItem> items = new List<BillItem>();
            string path = "Assets/Json/InvoicedOrdersRequestBody.json";
            string requestBodyJson = File.ReadAllText(path);
            JObject reqObj = JObject.Parse(requestBodyJson);
            reqObj["variables"]["input"]["id"] = id;

            //Sending Request
            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StringContent(reqObj.ToString(), encoding: Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue(_authToken);
            var response = await _httpClient.SendAsync(request);
            JObject responseObj = JObject.Parse(await response.Content.ReadAsStringAsync());
            JArray orderSets = (JArray)responseObj["data"]["invoiceDetails"]["Orders"];

            int orderCount = 0;
            foreach(var orderSet in orderSets)
            {
                JArray orders = (JArray)orderSet["Lines"];
                foreach(var order in orders)
                {
                    BillItem item = new BillItem();
                    item.Name = order["SKU"]["title"].ToString();
                    item.Quantity = int.Parse(order["quantity"].ToString());
                    item.Rate = Decimal.Parse(order["amountDetails"]["rate"].ToString());
                    decimal discountedAmount = Decimal.Parse(order["amountDetails"]["subTotal"].ToString());
                    decimal discountAmount = Decimal.Parse(order["amountDetails"]["discountAmount"].ToString());
                    item.DiscountRate = AmountHelperStatic.CalculateDiscountRate(discountedAmount + discountAmount, discountedAmount);
                    orderCount++;
                    item.SN = orderCount;
                    items.Add(item);
                }
            }
            return items;
        }
        
    }

    record Tab
    {
        public int Recieved { get; set; }

        public int Invoiced { get; set; }

        public int Dispatched { get; set; }

        public int Delivered { get; set; }

        public int Replaced { get; set; }
    }
}
