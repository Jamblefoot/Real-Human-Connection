using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchRandomizer : MonoBehaviour
{
    AudioSource audioSource;

    float min = 0.8f;
    float max = 1.2f;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void RandomizePitch()
    {
        audioSource.pitch = min + Random.value * (max - min);
    }
}
