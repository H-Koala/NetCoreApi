using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.Middleware
{
    /// <summary>
    /// 自定义中间件
    /// </summary>
    public class HttpContextMiddleware
    {
        private readonly ILogger Logger;
        private readonly RequestDelegate Next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="next"></param>
        public HttpContextMiddleware(ILogger<HttpContextMiddleware> logger, RequestDelegate next)
        {
            Logger = logger;
            Next = next;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context) {
            try {
               
            }
            catch (Exception) {
                Logger.LogError("HttpContextMiddleware 中间件执行异常");
            }
            
        }

    }
}
