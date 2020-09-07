using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using AspCoreApi.Controllers;
using AspCoreApi.Filter.Authorization;
using IServices.Authorization;
using IServices.ICommon.IRepository.IDapper;
using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using Services.Common.Repository.Dapper;
using Services.Interceptor;
using Services.RabbitMQ;
using System;
using System.Linq;
using System.Reflection;

namespace AspCoreApi.HelperTool
{
    /// <summary>
    /// 
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
    {
        /// <summary>
        /// Db 连接字符串
        /// </summary>
        public string DbContenct { get; set; }
        /// <summary>
        /// Db 类型
        /// </summary>
        public Services.Common.Db_Type.DbName DbType {get;set;}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            //1、注册拦截器
            //builder.Register(a => new CustomAOPInterception());
            builder.RegisterType<CustomAOPInterception>();
            //注入程序集下面的所有以Service结尾的，并且不是接口的类，注入到所实现的接口中
            builder.RegisterAssemblyTypes(Assembly.Load("IServices"), Assembly.Load("Services"))
             .Where(t => t.Name.EndsWith("Service"))
              .AsImplementedInterfaces().PropertiesAutowired().EnableInterfaceInterceptors();
            builder.Register(c => new SqlDapperService(DbContenct, DbType)).As<ISqlDapperService>().InstancePerLifetimeScope();
            builder.RegisterType<RabbitMQClient>().SingleInstance();
            //controller 屬性注入需要打開 將創建Controller 由Autofac 接管 支持 屬性注入需要打開
            var controllersTypesInAssembly = typeof(Startup).Assembly.GetExportedTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToArray();
            builder.RegisterTypes(controllersTypesInAssembly).PropertiesAutowired();  

        }
    }
}