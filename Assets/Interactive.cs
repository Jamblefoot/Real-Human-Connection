using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactive : MonoBehaviour
{
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallPressFunctions()
    {
        callOnPress.Invoke();
    }

    public void CallReleaseFunctions()
    {
        callOnRelease.Invoke();
        interactor = null;
    }
}
