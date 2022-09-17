using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditAction : MonoBehaviour
{
    [SerializeField]
    private Button _editButton; 
    [SerializeField]
    private GameObject _createSend;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _editButton.onClick.AddListener(ButtonClick);
    }

    void ButtonClick()
    {
        _createSend.SetActive(true);
    }
}
