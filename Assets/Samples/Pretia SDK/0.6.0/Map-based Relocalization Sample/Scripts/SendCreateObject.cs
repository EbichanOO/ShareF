using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCreateObject : MonoBehaviour
{
    public GameObject createobj;
    public GameObject parentcontents;
    private GameObject camera;
    private Vector3 mousePosition;
    private Vector3 mousePositionW;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
        mousePositionW = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 20));
        if (Input.GetMouseButtonUp(0) == true)
        {
            Vector3 vec = mousePositionW - camera.transform.position;
            GameObject cloneObj = Instantiate(createobj, mousePositionW, Quaternion.Euler(0, 0, 90), parentcontents.transform);
        }
        Debug.Log("mousePositoinW" + mousePositionW.x + ":" + mousePositionW.y + ":" + mousePositionW.z);
    }
}
