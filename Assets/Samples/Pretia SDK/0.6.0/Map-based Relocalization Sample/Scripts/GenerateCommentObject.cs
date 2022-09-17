using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using HttpHandler;

public class GenerateCommentObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var client = new HttpClient();
        HttpResponseMessage result = client.GetAsync(@"http://35.239.223.188:5000/init/object").Result;
        var resultBody = result.Content.ReadAsStringAsync().Result;
        Debug.Log(resultBody);

        for(int i=0; i< 2; i++) {
            // CubeプレハブをGameObject型で取得
            GameObject obj = (GameObject)Resources.Load("Cube");
            // Cubeプレハブを元に、インスタンスを生成
            Instantiate (obj, new Vector3(0.0f,(float)(0.2*(i+1)),0.0f), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
