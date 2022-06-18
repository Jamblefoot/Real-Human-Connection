using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    MeshRenderer rend;

    bool blinking = false;
    int blinks = 0;
    float delay = 1f;

    //public bool isOn = true;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.material.DisableKeyword("_EMISSION");
    }

    /*void Update()
    {
        if(isOn)
        {
            blinks = 2;
            if(!blinking)
                StartCoroutine(BlinkCo());
        }
        else
        {
            blinks = 0;
        }
    }*/

    public void Blink(int blinkCount, float timeDelay)
    {
        blinks += blinkCount;
        delay = timeDelay;
        if(!blinking)
        {
            StartCoroutine(BlinkCo());
        }
    }
    IEnumerator BlinkCo()
    {
        blinking = true;
        while(blinks > 0)
        {
            rend.material.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(delay);
            rend.material.DisableKeyword("_EMISSION");
            yield return new WaitForSeconds(delay);
        }

        blinking = false;
    }
}
