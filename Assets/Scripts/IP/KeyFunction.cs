using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class KeyFunction : MonoBehaviour
{
    public string keyName;
    public Interactable interactable;
    public NumberPadInput numberPadInput;

    private void OnValidate()
    {
        //interactable = GetComponent<Interactable>(); 
    }

    void Start()
    {
        keyName = gameObject.name;
        //numberPadInput = NumberPadInput.Instance; 
    }

    public void OnClick()
    {
        numberPadInput.OnKeyPressedEvent(keyName);
    }

    private void OnEnable()
    {
        interactable.OnClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        interactable.OnClick.RemoveListener(OnClick);
    }
}
