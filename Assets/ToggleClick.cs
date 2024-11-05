using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToggleClick : MonoBehaviour
{
    private CompressableButton button;
    private ToggleOnEvents toggleOnEvent;
    private ToggleOffEvents toggleOffEvent;
    bool state;
    bool StartExecuted = false;
    bool isChecking;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<CompressableButton>();
        toggleOnEvent = button.GetStateEvents<ToggleOnEvents>("ToggleOn");
        toggleOffEvent = button.GetStateEvents<ToggleOffEvents>("ToggleOff");
        state = toggleOnEvent.IsSelectedOnStart;
        // Debug.Log(state);
        StartExecuted = true;
        isChecking = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckState()
    {
        isChecking = true;
        if (StartExecuted)
        {
            if (state) toggleOnEvent.OnToggleOn.Invoke();
            else toggleOffEvent.OnToggleOff.Invoke();        
        }
    }

    public void ChangeState()
    {
        if (!isChecking)
        {
            state = !state;
        }
        isChecking = false;
    }
    
}