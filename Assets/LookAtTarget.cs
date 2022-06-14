using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;

    List<Transform> trans = new List<Transform>();
    
    // Start is called before the first frame update
    void Start()
    {
        if(target == null) return;

        FindChildren(target);

        StartCoroutine(Watch());
    }

    void FindChildren(Transform tran)
    {
        foreach(Transform child in tran)
        {
            trans.Add(child);
            FindChildren(child);
        }
    }

    IEnumerator Watch()
    {
        while(target != null)
        {
            transform.LookAt(trans[Random.Range(0, trans.Count)], Vector3.up);
            yield return new WaitForSeconds(Random.value * 2);
        }
    }
}
