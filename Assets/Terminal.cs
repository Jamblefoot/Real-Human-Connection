using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    public BlendshapeControl character;
    [SerializeField] RectTransform bar;
    Blinker blinker;

    AudioSource audioSource;
    [SerializeField] AudioSource secondaryAudio;
    [SerializeField] AudioClip tickSound;

    public int dataCount;

    bool makingNoise;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Room room = GetComponentInParent<Room>();
        character = room.GetComponentInChildren<BlendshapeControl>();

        blinker = GetComponentInChildren<Blinker>();
    }

    public float ComputeCompatibility(float[] setup)
    {
        float result = 0f;
        if(character == null) 
        {
            if(bar != null)
                bar.localScale = new Vector3(result, 1, 1);
            return result;
        }

        result = character.CheckCompatibility(setup);
        if (bar != null)
            bar.localScale = new Vector3(result, 1, 1);
        return result;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(dataCount > 0)
        {
            MakeNoise();
        }
    }
    public void MakeNoise()
    {
        if(!makingNoise)
            StartCoroutine(MakeNoiseCo());
    }

    IEnumerator MakeNoiseCo()
    {
        makingNoise = true;
        audioSource.Play();
        if(secondaryAudio == null)
            audioSource.PlayOneShot(tickSound);
        else secondaryAudio.Play();
        while(audioSource.isPlaying)
        {
            if(secondaryAudio != null && dataCount > 0)
            {
                if(secondaryAudio.isPlaying)
                {
                    yield return new WaitForSeconds(Random.Range(1, 3));

                    dataCount -= 1;
                    secondaryAudio.Pause();
                }
                else
                {
                    if(dataCount > 0)
                    {
                        secondaryAudio.Play();
                    }
                }
            }
            yield return null;
        }

        makingNoise = false;
    }
}
