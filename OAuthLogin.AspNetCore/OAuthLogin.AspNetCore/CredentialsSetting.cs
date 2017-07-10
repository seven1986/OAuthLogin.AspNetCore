namespace OAuthLogin.AspNetCore
{
    /// <summary>
    /// 第三方平台APP信息配置实体类
    /// </summary>
    public class CredentialsSetting
    {
        public CredentialSetting QQ { get; set; }
        public CredentialSetting Wechat { get; set; }
        public CredentialSetting Weibo { get; set; }
        public CredentialSetting FaceBook { get; set; }
        public CredentialSetting KaKao { get; set; }
    }

    public class CredentialSetting
    {
        /// <summary>
        /// AppKey
        /// </summary>
        public string client_id { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        public string client_secret { get; set; }
    }
}
