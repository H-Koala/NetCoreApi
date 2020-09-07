using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.HelperTool
{
    /// <summary>
    /// 全局null 处理
    /// </summary>
    public class CustomizeJsonOutputFormatter : JsonOutputFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializerSettings"></param>
        public CustomizeJsonOutputFormatter(JsonSerializerSettings serializerSettings) : base(serializerSettings, ArrayPool<char>.Shared)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public new JsonSerializerSettings SerializerSettings => base.SerializerSettings;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        protected override JsonWriter CreateJsonWriter(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            var jsonWriter = new NullJsonWriter(writer)
            {
                ArrayPool = new JsonArrayPool<char>(ArrayPool<char>.Shared),
                CloseOutput = false,
                AutoCompleteOnClose = false
            };
            return jsonWriter;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NullJsonWriter : JsonTextWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public NullJsonWriter(TextWriter textWriter) : base(textWriter)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public override void WriteNull()
        {
            this.WriteValue(String.Empty);
        }
    }
}
