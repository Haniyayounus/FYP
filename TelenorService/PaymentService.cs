using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TelenorService.Model;

namespace TelenorService
{
    public static class PaymentService
    {
        private static string _apiToken;
        private static int _duration;
        private static DateTime _lastToken;
        private const string BaseUrl = "https://apis.telenor.com.pk/";
        private const string ChargeApi = BaseUrl + "payment/v1/charge";
        private const string SmsApi = BaseUrl + "sms/v1/send";
        private const string AuthApi = BaseUrl + "oauthtoken/v1/generate?grant_type=client_credentials";
        static PaymentService()
        {
            GenerateToken();
        }
        public static async Task<ChargeResponse> ChargeUser(ChargeRequest request)
        {
            if (DateTime.Now.Subtract(_lastToken).TotalSeconds >= _duration || string.IsNullOrEmpty(_apiToken))
            {
                await GenerateToken();
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ChargeApi, data);
                var result = await response.Content.ReadAsAsync<ChargeResponse>();
                SmsRequest sms = new SmsRequest()
                {
                    recipientMsisdn = request.msisdn
                };
                if (result.Message == "Success")
                {
                    sms.messageBody =
                        "MEDIUS App mein khushamdeed. Subscription@Rs.35/week from mobile balance. Agla charge " +
                        DateTime.Now.AddDays(request.DurationInWeeks * 7).ToString("MMM dd") +
                        ". Istemal k liye bit.ly/MUSESub. Khatam k liye visit MUSE App Settings";
                }
                else //if (result.errorMessage == "The account balance is insufficient.")
                {
                    sms.messageBody =
                        "Sorry! MEDIUS Learning App subscription failed. Telenor mobile balance recharge kar kay dobara koshish kijiye bit.ly/MUSESub";
                }

#pragma warning disable 4014
                Task.Run(() =>
                {
                    SendSms(sms);
                }).ConfigureAwait(false);
#pragma warning restore 4014

                return result;
            }
        }

        public static async Task<SmsResponse> SendSms(SmsRequest request)
        {
            if (DateTime.Now.Subtract(_lastToken).TotalSeconds >= _duration || string.IsNullOrEmpty(_apiToken))
            {
                await GenerateToken();
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(request);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(new Uri(SmsApi), data);
                var result = await response.Content.ReadAsAsync<SmsResponse>();
                return result;
            }
        }

        public static async Task GenerateToken()
        {
            using (HttpClient client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes("cerw8aFhu47ffSBobATONKNqhAUa5RRn:IdF8eAqmmyS6uTPY");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsync(AuthApi, null);
                var result = await response.Content.ReadAsAsync<AuthResponse>();
                if (result != null && string.IsNullOrEmpty(result.access_token) == false &&
                    string.IsNullOrEmpty(result.expires_in) == false)
                {
                    _duration = 3500;
                    _apiToken = result.access_token;
                    _lastToken = DateTime.Now;
                }
            }
        }
    }
}
