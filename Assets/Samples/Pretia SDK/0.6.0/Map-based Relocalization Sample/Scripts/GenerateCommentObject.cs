using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
//using Unity.Plastic.Newtonsoft.Json.Linq;
//using Unity.Plastic.Newtonsoft.Json;


public class GenerateCommentObject : MonoBehaviour
{
    // Start is called before the first frame update
    private HttpResponseMessage result;
    private HttpClient client;
    private string resultBody;
    
    [SerializeField]
    private GameObject createobj;
    private List<string> resultlist;
    void Start()
    {
        Http();
        Debug.Log(resultBody);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public String Http()
    {
        client = new HttpClient();
        result = client.GetAsync(@"http://35.239.223.188:5000/get/object/all").Result;
        resultBody = result.Content.ReadAsStringAsync().Result;
        return resultBody;
    }

    void readJson()
    {
        

        
    }
}
