using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplyControl : MonoBehaviour
{
    Interactive interactive;

    HumanAI receiver;
    public int index = 0;

    public string[] options;

    public string terminalString;
    // Start is called before the first frame update
    void Start()
    {
        interactive = GetComponent<Interactive>();
        receiver = GetComponentInParent<HumanAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reply()
    {
        if(interactive.interactor != null)
        {
            interactive.interactor.Talk(options[Random.Range(0, options.Length)]);
        }

        if(receiver != null)
        {
            receiver.ReceiveReply(index);
        }

        if(terminalString != "")
        {
            InteractControl interactCon = FindObjectOfType<InteractControl>();
            if(interactCon != null)
            {
                interactCon.WriteToTerminal(terminalString);
                if(terminalString.Contains("-")) 
                    interactCon.RejectFeatures();
                else if(terminalString.Contains("+"))
                    interactCon.ApproveFeatures();

                GameControl.instance.playerInteractions += 1;
            }
        }
    }
}
