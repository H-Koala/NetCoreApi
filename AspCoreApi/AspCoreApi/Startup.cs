using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspCoreApi.Filter.Action;
using AspCoreApi.Filter.Exception;
using AspCoreApi.HelperTool;
using AspCoreApi.Middleware;
using AspCoreApi.Middleware.HttpHeaders;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Services.Common;
using Services.Common.Encryption;
using StackExchange.Profiling;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AspCoreApi
{
    public class Startup
    {
        /// <summary>
        /// 構造函數
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        private IConfiguration Configuration { get; }
        private static readonly string swaggerDocName = "v1";
        /// <summary>
        /// 运行时调用此方法。使用此方法向容器添加服务。
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
            //设置代理
            //services.AddHttpClient("configured-inner-handler");
            //.ConfigurePrimaryHttpMessageHandler(() =>
            //{
            //    return new HttpClientHandler()
            //    {
            //        // AllowAutoRedirect = false,
            //        // UseDefaultCredentials = true,
            //        Proxy = new WebProxy(Configuration["Proxy"]),
            //        UseProxy = true
            //    };
            //});

            //添加jwt验证
            if (Convert.ToBoolean(Configuration["JWT"]))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateLifetime = true,//是否验证失效时间
                       ClockSkew = TimeSpan.FromSeconds(5),
                       ValidateAudience = true,//是否验证Audience
                       AudienceValidator = (m, n, z) =>
                       {
                           //这里采用动态验证的方式，在重新登陆时，刷新token，旧token就强制失效了
                           return m != null && true;
                       },
                       ValidateIssuer = true,//是否验证Issuer
                       ValidIssuer = Configuration["Domain"],//Issuer，这两项和前面签发jwt的设置一致

                       ValidateIssuerSigningKey = true,//是否验证SecurityKey
                       IssuerSigningKey = new SymmetricSecurityKey(Encryption.DecryptBytes(Configuration["SecurityKey"]))//拿到SecurityKey
                   };
                   options.Events = new JwtBearerEvents
                   {
                       //此处为权限验证失败后触发的事件
                       OnChallenge = context =>
                       {
                           //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要，必须
                           //自定义自己想要返回的数据结果
                           //var payload = new JsonResult(
                           //        new ReturnMessageModel()
                           //        {
                           //            Status = 1,
                           //            Message = ""
                           //        }
                           //    );
                           if (!string.IsNullOrEmpty(context.ErrorDescription))
                           {
                               context.HandleResponse();
                               string payload = JsonConvert.SerializeObject(new { Status = 3, Message = context.ErrorDescription, Code = "", Data = "" });
                               //自定义返回的数据类型
                               context.Response.ContentType = "application/json";
                               //自定义返回状态码，默认为401 这里改成 200
                               context.Response.StatusCode = StatusCodes.Status200OK;
                               //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                               //输出Json数据结果
                               context.Response.WriteAsync(payload);
                           }
                           return Task.FromResult(0);
                       }
                   };
               });
            }
            //从容器中创建控制器的构造函数参数，控制器本身并不是从容器中创建的。要想让控制器也从容器中创建需要使用
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            services.AddMvc(
                options =>
                {
                    options.Filters.Add<GlobalExceptionFilter>();//全局異常過濾器
                    options.Filters.Add<CustomActionFilterAttribute>();//全局Action 過濾器
                }
                ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    opts.SerializerSettings.ContractResolver = new DefaultContractResolver();//和Model
                }
                );
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
            // 添加MiniProfiler服务
            services.AddMiniProfiler(options =>
            {
                // 设定弹出窗口的位置是左下角
                options.PopupRenderPosition = RenderPosition.BottomLeft;
                // 设定在弹出的明细窗口里会显式Time With Children这列
                options.PopupShowTimeWithChildren = true;
                // 设定访问分析结果URL的路由基地址
                options.RouteBasePath = "/profiler";
            }
            ).AddEntityFramework();
            //services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {

                    builder.AllowAnyOrigin() //允许任何来源的主机访问

                    .AllowAnyMethod()

                    .AllowAnyHeader()

                    .AllowCredentials();//指定处理cookie

                });

            });
            services.AddSwaggerGen(c =>
            {

                var info = new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Web API",
                    Description = "ASP.NET Core Web API",
                };
                Uri contactUrl = null;//: new Uri(s)
                info.Contact = new OpenApiContact { Name = "hnj", Email = "1092454106@qq.com", Url = contactUrl };
                c.SwaggerDoc(swaggerDocName, info);
                //获取应用程序所在目录（绝对，不受工作目录影响，采用此方法获取路径）
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "AspCoreApi.xml");
                c.IncludeXmlComments(xmlPath, true);//如果需要显示控制器注释需将第二个参数设置为true
                var xmlModelPath = Path.Combine(basePath, "Model.xml");
                c.IncludeXmlComments(xmlModelPath);
                // 在接口类、方法标记属性 [HiddenApi]，可以阻止【Swagger文档】生成
                //c.DocumentFilter<HiddenApiFilter>(); 
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                //给api添加token令牌证书
                var security = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new[] { "readAccess", "writeAccess" }
                    }
                };
                //添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致
                c.AddSecurityRequirement(security);
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });
            //消息监听
            //services.AddHostedService<"写自己的监听类">();
            //services.AddSingleton<RabbitMQClient, RabbitMQClient>(); 换成AutoFac注册
            services.AddMemoryCache();
            //使用Autofac實現IOC
            var containerBuilder = new ContainerBuilder();
            //模塊化注入
            //containerBuilder.RegisterModule<AutofacModuleRegister>();
            containerBuilder.RegisterModule(new AutofacModuleRegister()
            {
                DbContenct = Configuration["ConnectionStrings:dbtest"],
                DbType = Db_Type.DbName.Oracle
            });
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        /// <summary>
        /// 运行时调用此方法。使用此方法配置HTTP请求管道。
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //添加jwt验证
            if (Convert.ToBoolean(Configuration["JWT"]))
            {
                app.UseAuthentication();
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseSecurityHeadersMiddleware(
              new SecurityHeadersBuilder()
                  .AddDefaultSecurePolicy());
            //var provider = new FileExtensionContentTypeProvider();
            //provider.Mappings[".mp4"] = "application/octet-stream";
            app.UseStaticFiles();
            //指定静态文件路径
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(@"D:\Upload"),
            //    RequestPath = "/static",
            //    // ContentTypeProvider = provider
            //});
            app.UseCors("any");
            if (Convert.ToBoolean(Configuration["Swagger"]))
            {
                app.UseMiniProfiler();
                //開發以及測試環境才打開Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DocExpansion(DocExpansion.None);//折叠
                    c.RoutePrefix = string.Empty;//设置后直接输入IP就可以进入接口文档
                    c.SwaggerEndpoint(
                       url: $"swagger/{swaggerDocName}/swagger.json", //这里一定要使用相对路径，不然网站发布到子目录时将报告："Not Found /swagger/v1/swagger.json"                                           // description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                       name: "Web Api"
                   );
                    c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("AspCoreApi.index.html");
                    //c.DefaultModelsExpandDepth(-1); DefaultModelsExpandDepth设置为-1 可不显示models
                });
            }
            app.UseMvc();
        }
    }
}
