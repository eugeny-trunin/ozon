using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Filters
{
    public class TextReportOutputFormatter : TextOutputFormatter
    {
        public TextReportOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Plain));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(ITextReport).IsAssignableFrom(type)
                || typeof(IEnumerable<ITextReport>).IsAssignableFrom(type);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                if (context.Object is IEnumerable<ITextReport>)
                    return writer.WriteAsync(string.Join("\n\n", ((IEnumerable<ITextReport>)context.Object).Select(r => r.GetReport())));
                else
                    return writer.WriteAsync(((ITextReport)context.Object)?.GetReport());
            }
        }
    }
}
