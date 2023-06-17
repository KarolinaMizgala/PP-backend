using PayPal.Api;

namespace WizardShopAPI.Paypal
{
    public static class PaypalConfiguration
    {
        //Variables for storing the clientID and clientSecret key  
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        //Constructor  
        static PaypalConfiguration()
        {
            var config = GetConfig();
            ClientId = "Abzaato61pg-mPAX7EVuJT9V33H-_Q9iTiSLWD8qIt2oeu6MRyF9tVcevaD5Ui7KAowl4Yfubhi3-SMw";
            ClientSecret = "EITMJp-_IsZpQuZgX8IF1Jmaj_B9EB63edQsMBEVZTPj_xZStTMqUOzFHsRmRZwxtyJ2Mrk9m2jKB-kC";
        }
        // getting properties from the web.config  
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        private static string GetAccessToken()
        {
            // getting accesstocken from paypal  
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}
