using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Specialized;

namespace UberClone.Helpers
{
   public class RestHelper
    {
        public static async Task<Tuple<T, bool,string>>APIRequest<T>(string url, HttpVerbs method = HttpVerbs.GET, NameValueCollection getParameters = null, FormUrlEncodedContent postParameters = null,FormUrlEncodedContent putParameters =null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //setup client
                    Uri uri = new Uri(url);
                    client.BaseAddress = uri;
                    client.DefaultRequestHeaders.Accept.Clear();

                    //make request
                    HttpResponseMessage response = new HttpResponseMessage();
                    switch (method)
                    {
                        case HttpVerbs.GET:
                            uri = uri.AttachParameters(getParameters);
                            response = await client.GetAsync(uri);
                            break;
                        case HttpVerbs.POST:
                            response = await client.PostAsync(uri, postParameters);
                            break;
                        case HttpVerbs.PUT:
                            response = await client.PutAsync(uri, putParameters);
                            break;
                        default:
                            break;
                    }
                    if (!response.IsSuccessStatusCode)
                    {
                        return new Tuple<T, bool, string>(default(T), false, response.ReasonPhrase);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var stringResponseJson = await response.Content.ReadAsStringAsync();

                        T result = JsonConvert.DeserializeObject<T>(stringResponseJson);

                        return new Tuple<T, bool, string>(result, true, response.ReasonPhrase);
                    }
                    else
                    {
                        return new Tuple<T, bool, string>(default(T), false, "Error In API Request");
                    }

                    
                }
            }
            catch (Exception ex)
            {
                return new Tuple<T, bool, string>(default(T), false,"Exception: "+ex.InnerException.Message);
            }
        }

    }

    public static class RESTExtensions
    {
        /*
                     var postParameters = new FormUrlEncodedContent(new[]
                {
                     new KeyValuePair<string, string>("login", username),
                     new KeyValuePair<string, string>("password", password),
                 });
        */
        public static Uri AttachParameters(this Uri uri, NameValueCollection parameters)
        {
            if (parameters?.Count > 0)
            {
                var stringBuilder = new StringBuilder();
                string str = "?";
                for (int index = 0; index < parameters.Count; ++index)
                {
                    stringBuilder.Append(str + parameters.AllKeys[index] + "=" + parameters[index]);
                    str = "&";
                }
                return new Uri(uri + stringBuilder.ToString());
            }
            else
            {
                return uri;
            }
        }
    }

}