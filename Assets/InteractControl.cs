using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractControl : MonoBehaviour
{

    [SerializeField] LineRenderer lineRend;

    [SerializeField] LayerMask interactLayers;
    [SerializeField] Terminal terminal;
    [SerializeField] Text terminalText;

    [SerializeField] TextMesh speechText;
    bool talking;

    public Interactive currentInteractive;

    public Vector3 interactPoint = Vector3.zero;

    BlendshapeControl bsc;

    bool checking;

    public struct BodyPartParams
    {
        public int index;
        public float value;
    }
    List<BodyPartParams> bodyPartData;

    RoomControl roomControl;
    // Start is called before the first frame update
    void Start()
    {

        if(lineRend != null)
            lineRend.positionCount = 0;

        speechText.text = "";

        bsc = GetComponentInParent<BlendshapeControl>();

        roomControl = FindObjectOfType<RoomControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            Vector3 lineEnd = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 20f, interactLayers, QueryTriggerInteraction.Collide))
            {
                Interactive interactive = hit.collider.GetComponent<Interactive>();
                if(interactive != null)
                {
                    //Debug.Log("Hitting interactive trigger for " + hit.collider.gameObject.name);
                    lineEnd = hit.point;
                    interactPoint = hit.point;
                    if(currentInteractive != interactive)
                    {
                        currentInteractive = interactive;
                        interactive.interactor = this;
                        interactive.CallPressFunctions();

                        if(interactive.options.Length > 0 && Input.GetButtonDown("Fire1"))
                        {
                            if(!checking)
                                StartCoroutine(CheckIfLike(interactive, interactive.GetComponentInParent<HumanAI>()));
                            /*for(int i = 0; i < interactive.options.Length; i++)
                            {
                                terminalText.text += "\n" + interactive.options[i].terminalName;
                                terminal.dataCount += 1;
                            }*/
                        }
                    }
                }
            }
            else
            {
                lineEnd = transform.position + ray.direction * 20f;
                if(currentInteractive != null)
                {
                    currentInteractive.CallReleaseFunctions();
                    currentInteractive.interactor = null;
                    currentInteractive = null;
                    
                }

                interactPoint = transform.position + ray.direction * 1.5f;//Vector3.zero;
            }

            if((interactPoint - transform.position).sqrMagnitude > 6)
                interactPoint = transform.position + ray.direction * 1.5f;

            if(lineRend != null)
            {
                lineRend.positionCount = 2;
                lineRend.SetPosition(0, lineRend.transform.position);
                lineRend.SetPosition(1, lineEnd);
            }
        }
        else
        {
            if(lineRend != null)
            {
                lineRend.positionCount = 0;
            }

            if(currentInteractive != null)
            {
                currentInteractive.CallReleaseFunctions();
                currentInteractive.interactor = null;
                currentInteractive = null;
            }

            interactPoint = Vector3.zero;
            
        }
    }

    public void Talk(string words)
    {
        if(!talking)
            StartCoroutine(TalkCo(words));
    }
    IEnumerator TalkCo(string words)
    {
        talking = true;
        bsc.talking = true;
        speechText.text = words;
        yield return new WaitForSeconds(words.Length);

        talking = false;
        bsc.talking = false;
        speechText.text = "";

    }

    IEnumerator CheckIfLike(Interactive interactive, HumanAI ai)
    {
        checking = true;

        HumanAI.ReplySetup[] replies = new HumanAI.ReplySetup[2];
        HumanAI.ReplySetup reply1 = new HumanAI.ReplySetup();
        reply1.label = "Yes";
        reply1.code = 5;
        reply1.terminalLabel = interactive.terminalName + "++";
        replies[0] = reply1;
        HumanAI.ReplySetup reply2 = new HumanAI.ReplySetup();
        reply2.label = "No";
        reply2.code = 6; 
        reply2.terminalLabel = interactive.terminalName + "--";
        replies[1] = reply2;
        ai.AddThingToSay("Do you like my " + interactive.displayName, replies);
        /*ai.Say("Do you like my " + interactive.displayName);
        ai.SetupReply("Yes", 5, interactive.terminalName + "++");
        ai.SetupReply("No", 6, interactive.terminalName + "--");
        HumanAI.ReplySetup[] replies = new HumanAI.ReplySetup[2];
        ai.AddThingToSay("Do you like my " + interactive.displayName, replies);*/

        if(interactive.options.Length > 0)
        {
            bodyPartData = new List<BodyPartParams>();
            for(int i = 0; i < interactive.options.Length; i++)
            {
                BodyPartParams part = new BodyPartParams();
                part.index = bsc.GetBlendShapeIndex(interactive.options[i].blendShapeName + " Max");
                part.value = ai.GetBSC().GetBlendShapeWeight(part.index);
                bodyPartData.Add(part);
                BodyPartParams partMin = new BodyPartParams();
                partMin.index = bsc.GetBlendShapeIndex(interactive.options[i].blendShapeName + " Min");
                partMin.value = ai.GetBSC().GetBlendShapeWeight(partMin.index);
                bodyPartData.Add(partMin);
            }
        }

        yield return new WaitForSeconds(4f);

        while(ai.IsTalking() && checking)
        {
            yield return null;
        }

        //terminalText.text += "\n" + interactive.terminalName;
        //terminal.dataCount += 1;
        checking = false;
    }

    public void WriteToTerminal(string words)
    {
        terminalText.text += "\n" + words;
        terminal.dataCount += 1;
    }

    public void ApproveFeatures()
    {
        if(bodyPartData == null) return;

        for(int i = 0; i < bodyPartData.Count; i++)
        {
            bsc.AdjustDesired(bodyPartData[i].index, bodyPartData[i].value, 1);
        }
        bodyPartData.Clear();

        if(roomControl != null) roomControl.UpdateTerminal();
    }
    public void RejectFeatures()
    {
        if (bodyPartData == null) return;

        for (int i = 0; i < bodyPartData.Count; i++)
        {
            bsc.AdjustDesired(bodyPartData[i].index, bodyPartData[i].value, -1);
        }
        bodyPartData.Clear();

        if (roomControl != null) roomControl.UpdateTerminal();
    }

    public void ResetForNextRoom()
    {
        checking = false;
    }
}
