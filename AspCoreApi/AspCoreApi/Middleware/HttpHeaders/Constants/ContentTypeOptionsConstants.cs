namespace AspCoreApi.Middleware.HttpHeaders.Constants
{
    /// <summary>
    ///與X-Content-Type-Options相關的常量
    /// </summary>
    public static class ContentTypeOptionsConstants
    {
        /// <summary>
        /// X-Content-Type-Options的標題值
        /// </summary>
        public static readonly string Header = "X-Content-Type-Options";

        /// <summary>
        /// 禁用內容嗅探
        /// </summary>
        public static readonly string NoSniff = "nosniff";
        
    }
}