using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractControl : MonoBehaviour
{
    [SerializeField] LineRenderer lineRend;

    [SerializeField] LayerMask interactLayers;
    [SerializeField] Text terminalText;

    Interactive currentInteractive;
    // Start is called before the first frame update
    void Start()
    {
        if(lineRend != null)
            lineRend.positionCount = 0;
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
                    Debug.Log("Hitting interactive trigger for " + hit.collider.gameObject.name);
                    lineEnd = hit.point;
                    if(currentInteractive != interactive)
                    {
                        currentInteractive = interactive;
                        interactive.CallPressFunctions();

                        if(interactive.options.Length > 0)
                        {
                            for(int i = 0; i < interactive.options.Length; i++)
                            {
                                terminalText.text += "\n" + interactive.options[i].terminalName;
                            }
                        }
                    }
                }
            }
            else
            {
                lineEnd = transform.position + ray.direction * 20f;
            }

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

            currentInteractive = null;
        }
    }
}
