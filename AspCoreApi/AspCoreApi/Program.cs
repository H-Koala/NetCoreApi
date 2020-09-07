using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace AspCoreApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration((webHostBuilderContext, configurationbuilder) =>
           {
               var env = webHostBuilderContext.HostingEnvironment;//通过WebHostBuilderContext类的HostingEnvironment属性得到IHostingEnvironment接口对象
               configurationbuilder.SetBasePath(env.ContentRootPath)
             .AddJsonFile("appsettings.json", false, true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)//appsettings.{env.EnvironmentName}.json根据当前的运行环境来加载相应的appsettings文件
             .AddEnvironmentVariables();
           })
           .UseUrls("http://*:2600")
               .UseStartup<Startup>().UseKestrel(o => {
                   o.Limits.MaxRequestBodySize = null;
               })
          .UseNLog();
    }
}
