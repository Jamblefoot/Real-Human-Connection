using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactive : MonoBehaviour
{
    public string displayName = "";
    public string terminalName = "";
    [System.Serializable]
    public class Option
    {
        public string blendShapeName;
        public string displayName;
        public string terminalName;
        public bool used;
    }
    public Option[] options; 

    public UnityEvent callOnPress;
    public UnityEvent callOnRelease;

    public InteractControl interactor;

    public bool pointAtMe;

    public void CallPressFunctions()
    {
        if(Input.GetButtonDown("Fire1"))
            callOnPress.Invoke();
    }

    public void CallReleaseFunctions()
    {
        callOnRelease.Invoke();
        interactor = null;
    }
}
