using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRandomizer : MonoBehaviour
{
    [SerializeField] GameObject[] texts;
    [SerializeField] string[] options;
    // Start is called before the first frame update
    void Start()
    {
        //TextMesh textMesh = GetComponent<TextMesh>();
        //if(textMesh != null)
        //    textMesh.text = options[Random.Range(0, options.Length)];

        
        for(int i = 0; i < texts.Length; i++)
        {
            texts[i].SetActive(false);
        }
        texts[Random.Range(0, texts.Length)].SetActive(true);

        Destroy(this);
    }
}
