using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHeight : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if(target == null) return;

        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = pos;
    }
}
