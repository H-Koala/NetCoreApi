
using AspCoreApi.Middleware.HttpHeaders;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public static  class MiddlewareExtensions
    {
        /// <summary>
        /// 自定义Http 中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpContextMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpContextMiddleware>();
        }
        /// <summary>
        ///  自定义UseSecurityHeaders 中间件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, SecurityHeadersBuilder builder)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return app.UseMiddleware<SecurityHeadersMiddleware>(builder.Build());
        }
    }
}
