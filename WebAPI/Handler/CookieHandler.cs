using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.Handler
{
    /// <summary>
    /// 處理Request Respond 的Cookie, 如果本來沒有則用guid 產出ID作辨識
    /// </summary>
    public class CookieHandler : DelegatingHandler
    {
        /// <summary>
        /// Cookie 的key, 為字串"Id"
        /// </summary>
        public static string Id { get; set; } = "Id";

        /// <summary>
        /// 用家提出請求(request)並取得cookie
        /// </summary>
        /// <param name="request">client發送的請求</param>
        /// <param name="cancellationToken">如果取消則得到cancellationtoken</param>
        /// <returns></returns>
        async protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string IdValue;

            // Try to get the ID from the request; otherwise create a new ID.
            var cookie = request.Headers.GetCookies(Id).FirstOrDefault();
            if (cookie == null)
            {
                IdValue = Guid.NewGuid().ToString();
            }
            else
            {
                IdValue = cookie[Id].Value;
                try
                {
                    Guid guid = Guid.Parse(IdValue);
                }
                catch (FormatException)
                {
                    // Bad session ID. Create a new one.
                    IdValue = Guid.NewGuid().ToString();
                }
            }

            // Store the session ID in the request property bag.
            request.Properties[Id] = IdValue;

            // Continue processing the HTTP request.
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            // Set the session ID as a cookie in the response message.
            response.Headers.AddCookies(new CookieHeaderValue[] 
            {
            new CookieHeaderValue(Id, IdValue)
        });

            return response;
        }
    }
}