using UnityEngine;
using PretiaArCloud;
using System;

public class ARContentHandler : MonoBehaviour
{

    [SerializeField]
    private ARSharedAnchorManager _relocManager;

    [SerializeField]
    private GameObject _arContents;

    [SerializeField]
    private GameObject _gameObject;

    [SerializeField]
    private GameObject _editButton;

    void OnEnable()
    {
        _relocManager.OnMapRelocalized += OnMapRelocalized;
        _relocManager.OnRelocalized += OnRelocalized;
    }

    void OnDisable()
    {
        _relocManager.OnMapRelocalized -= OnMapRelocalized;
        _relocManager.OnRelocalized -= OnRelocalized;
    }

    private void OnRelocalized()
    {
        _arContents.SetActive(true);
        _gameObject.SetActive(true);
        _editButton.SetActive(true);
    }

    private void OnMapRelocalized(string mapKey)
    {
        Debug.Log($"Successfully relocalized {mapKey}");
    }
}