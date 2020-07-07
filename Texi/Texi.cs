using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Texi
{
    public class Texi
    {
        string url;
        HttpClient client;
        string apiKey;

        public Texi(string url, string apiKey)
        {
            this.url = url;
            this.apiKey = apiKey;
            this.client = new HttpClient();
        }

        public async void Send(string recipient, string message)
        {
            var uri = new Uri(url + "/api/send");
            var data = ToParameters(new { apikey = apiKey, recipient = recipient, message = message });
            var content = new FormUrlEncodedContent(data);
            var response = await client.PostAsync(uri, content);
            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(responseContent);
                if (o["message"].ToString() == "OK") {
                    var m = new Message {
                        Id = o["data"]["message_id"].ToString()
                    };
                    OnResponseReceived(new Response { Data = m });
                } else {
                    OnResponseReceived(new Response { Data = o["message"].ToString() });
                }
            }
        }

        public event EventHandler<ResponseEventArgs> ResponseReceived;

        protected virtual void OnResponseReceived(Response e)
        {
            OnResponseReceived(new ResponseEventArgs(e));
        }

        protected virtual void OnResponseReceived(ResponseEventArgs e)
        {
            if (ResponseReceived != null) {
                ResponseReceived(this, e);
            }
        }

        List<KeyValuePair<string, string>> ToParameters(object obj)
        {
            var parameters = new List<KeyValuePair<string, string>>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj)) {
                object value = property.GetValue(obj);
                parameters.Add(new KeyValuePair<string, string>(property.Name, value.ToString()));
            }
            return parameters;
        }
    }

    public class Message
    {
        public string Id { get; set; }
    }

    public class ResponseEventArgs : EventArgs
    {
        public Response Response { get; set; }

        public ResponseEventArgs(Response response)
        {
            this.Response = response;
        }
    }

    public class Response
    {
        public object Data { get; set; }
    }
}
