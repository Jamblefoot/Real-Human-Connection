using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;

    List<Transform> trans = new List<Transform>();

    MeshRenderer rend;

    int pupilSizeID;
    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.material.color = new Color(Random.value, Random.value, Random.value);
        pupilSizeID = Shader.PropertyToID("_PupilSize");

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

    void Update()
    {
        rend.material.SetFloat(pupilSizeID, Mathf.PerlinNoise(Time.time * 0.3f, Time.time * 0.3f) * 0.2f);
    }
}
