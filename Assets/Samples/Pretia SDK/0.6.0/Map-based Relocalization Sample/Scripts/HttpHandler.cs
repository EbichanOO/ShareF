using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

//public class HttpHandler : MonoBehaviour
namespace HttpHandler
{
    public class HttpHandlerFunctions{
        public string GetAllObjects() {
            var client = new HttpClient();
            HttpResponseMessage result = client.GetAsync(@"http://35.239.223.188:5000/get/object/all").Result;
            var resultBody = result.Content.ReadAsStringAsync().Result;
            return resultBody;
        }

        public void AddObject(string context, float x, float y, float z, float xTurn, float yTurn) {
            var param = new Dictionary<string, object>()
            {
                ["context"] = context,
                ["x"] = x,
                ["y"] = y,
                ["z"] = z,
                ["x-turn"] = xTurn,
                ["y-turn"] = yTurn
            };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(param);
            var content = new StringContent(jsonString, Encoding.UTF8, @"application/json");
            // 以下POST通信
            var client = new HttpClient();
            HttpResponseMessage result = client.PostAsync(@"http://35.239.223.188:5000/add/object", content).Result;
        }
    }
}
