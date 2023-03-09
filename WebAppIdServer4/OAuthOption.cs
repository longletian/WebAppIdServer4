namespace WebAppIdServer4
{
    public class OAuthOption
    {
        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端密钥
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 客户端回调地址（与后台设置一致）
        /// </summary>
        public string CallbackPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Scopes { get; set; }
    }
}
