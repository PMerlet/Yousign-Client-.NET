using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using Yousign_Client_.NET.YousignCosignService;

namespace Yousign_Client_.NET
{
    public sealed class CosignClient
    {
        private readonly string _signatureUrl;
        private readonly string _cosignUrl;
        private readonly string _key;
        private readonly string _username;
        private readonly string _passowrd;

        /// <summary>
        /// Instanciate a cosign client
        /// </summary>
        /// <param name="key">Yousign API key</param>
        /// <param name="username">Yousign API username</param>
        /// <param name="password">Yousign API password</param>
        /// <param name="isProd">Flag to set PROD or DEMO environment</param>
        public CosignClient(string key, string username, string password, bool isProd)
        {
            _key = key;
            _username = username;
            _passowrd = password;
            _cosignUrl = isProd ? Constants.CosignUrl : Constants.DemoCosignUrl;
            _signatureUrl = isProd ? Constants.SignatureUrl : Constants.DemoSignatureUrl;
        }

        /// <summary>
        /// Execute a request to cosign API
        /// </summary>
        /// <typeparam name="TResult">Type of return</typeparam>
        /// <param name="handler">Method to execute</param>
        /// <returns>TResult</returns>
        public TResult Execute<TResult>(Func<CosignWSClient, TResult> handler)
        {
            var binding = GetCosignBindingConfig();
            var endpoint = new EndpointAddress(_cosignUrl);

            using (var client = new CosignWSClient(binding, endpoint))
            {
                using (new OperationContextScope(client.InnerChannel))
                {
                    var apikeyHeader = MessageHeader.CreateHeader("apikey", string.Empty, _key);
                    OperationContext.Current.OutgoingMessageHeaders.Add(apikeyHeader);
                    var usernameHeader = MessageHeader.CreateHeader("username", string.Empty, _username);
                    OperationContext.Current.OutgoingMessageHeaders.Add(usernameHeader);
                    var passwordHeader = MessageHeader.CreateHeader("password", string.Empty,
                        PasswordHelper.GetPasswordHash(_passowrd));
                    OperationContext.Current.OutgoingMessageHeaders.Add(passwordHeader);

                    return handler(client);
                }
            }
        }

        /// <summary>
        /// Get URL to sign document
        /// </summary>
        /// <param name="token"></param>
        /// <param name="urlSuccess"></param>
        /// <param name="urlCancel"></param>
        /// <param name="urlCallback"></param>
        /// <returns>URL</returns>
        public string GetSignatureUrl(string token, string urlSuccess, string urlCancel, string urlCallback)
        {
            var url = string.Format("{0}{1}?urlsuccess={2}&urlcancel={3}&urlcallback={4}", _signatureUrl,
                token, HttpUtility.UrlEncode(urlSuccess), HttpUtility.UrlEncode(urlCancel),
                HttpUtility.UrlEncode(urlCallback));
            return url;
        }

        private static BasicHttpBinding GetCosignBindingConfig()
        {
            var binding = new BasicHttpBinding();
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.MaxReceivedMessageSize = Constants.MaxSize;
            binding.MaxBufferSize = Constants.MaxSize;
            binding.MaxBufferPoolSize = Constants.MaxSize;
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.ReaderQuotas.MaxDepth = Constants.MaxDepth;
            binding.ReaderQuotas.MaxArrayLength = Constants.MaxSize;
            binding.ReaderQuotas.MaxStringContentLength = Constants.MaxSize;
            return binding;
        }
    }
}
