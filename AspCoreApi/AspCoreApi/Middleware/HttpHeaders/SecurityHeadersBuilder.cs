
using AspCoreApi.Middleware.HttpHeaders.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.Middleware.HttpHeaders
{

    /// <summary>
    /// 
    /// </summary>
    public class SecurityHeadersBuilder
    {
        private readonly SecurityHeadersPolicy _policy = new SecurityHeadersPolicy();

        /// <summary>
        /// 一年中的秒數
        /// </summary>
        public const int OneYearInSeconds = 60 * 60 * 24 * 365;

        /// <summary>
        /// 按照最安全的方法添加默認標題
        /// </summary>
        public SecurityHeadersBuilder AddDefaultSecurePolicy()
        {
            AddContentTypeOptionsNoSniff();
            return this;
        }
        /// <summary>
        /// 將X-Content-Type-Options nosniff添加到所有請求
        /// 可以設置為防止MIME類型混淆攻擊
        /// </summary>
        public SecurityHeadersBuilder AddContentTypeOptionsNoSniff()
        {
            _policy.SetHeaders[ContentTypeOptionsConstants.Header] = ContentTypeOptionsConstants.NoSniff;
            return this;
        }


        /// <summary>
        /// 從所有請求中刪除標題
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public SecurityHeadersBuilder RemoveHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException(nameof(header));
            }

            _policy.RemoveHeaders.Add(header);
            return this;
        }

        /// <summary>
        /// <see cref="SecurityHeadersPolicy"/> 
        /// </summary>
        /// <returns> <see cref="SecurityHeadersPolicy"/>.</returns>
        public SecurityHeadersPolicy Build()
        {
            return _policy;
        }
    }
}
