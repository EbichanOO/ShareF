using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCommentObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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
