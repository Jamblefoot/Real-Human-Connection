using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riser : MonoBehaviour
{

    float maxHeight = 100;
    float speed = 1f;

    float moveAmount = 0;

    // Update is called once per frame
    void Update()
    {
        if(moveAmount < maxHeight)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            moveAmount += speed * Time.deltaTime;
        }
    }
}
