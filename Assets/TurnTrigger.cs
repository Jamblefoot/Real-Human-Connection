using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTrigger : MonoBehaviour
{
    BlendshapeControl guy;

    [SerializeField] int turnDirection = 1;
    // Start is called before the first frame update
    void Start()
    {
        guy = GetComponentInParent<Room>().GetComponentInChildren<BlendshapeControl>();

        if(guy == null)
            Destroy(gameObject);
    }

    public void Turn()
    {
        Debug.Log("Turn trigger should be firing");
        guy.Turn(turnDirection);
    }
}
