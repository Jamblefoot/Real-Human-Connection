using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    RoomControl roomControl;
    public Interactive button;

    // Start is called before the first frame update
    void Start()
    {
        roomControl = FindObjectOfType<RoomControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveRandom()
    {
        roomControl.MoveRandom();
    }
}
