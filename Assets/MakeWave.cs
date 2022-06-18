using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWave : MonoBehaviour
{
    BlendshapeControl bsc;
    // Start is called before the first frame update
    void Start()
    {
        bsc = GetComponent<BlendshapeControl>();
        bsc.waving = true;
    }
}
