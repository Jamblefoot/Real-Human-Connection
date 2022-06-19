using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string mixerName;

    Slider slider;

    public void SetVolume(float sliderValue)
    {
        audioMixer.SetFloat(mixerName, Mathf.Log10(sliderValue) * 20);
    }

    public void SetVolume()
    {
        if(slider == null)
            slider = GetComponent<Slider>();

        audioMixer.SetFloat(mixerName, Mathf.Log10(slider.value) * 20);
    }

    void Start()
    {
        slider = GetComponent<Slider>();
        audioMixer.SetFloat(mixerName, Mathf.Log10(GetComponent<Slider>().value) * 20);
    }
}