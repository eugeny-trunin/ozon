using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Filters
{
    public class ToStringOutputFormatter : TextOutputFormatter
    {
        public ToStringOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Plain));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return writer.WriteAsync(context.Object?.ToString());
            }
        }
    }
}
