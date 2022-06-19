using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public static SoundLibrary instance;

    [SerializeField] AudioSource voiceSource;
    [SerializeField] AudioSource staticSource;

    [System.Serializable]
    public class SoundParams
    {
        public string name;
        public AudioClip[] clips;
    }

    [SerializeField] SoundParams[] sounds;

    List<string> wordsToSay = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        if(SoundLibrary.instance != null)
            DestroyImmediate(this);
        else SoundLibrary.instance = this;
    }

    void Update()
    {
        if(wordsToSay.Count > 0)
        {
            //if(!staticSource.gameObject.activeSelf)
            //    staticSource.gameObject.SetActive(true);
            if(!staticSource.isPlaying)
                staticSource.Play();
            if(!voiceSource.isPlaying)
            {
                AudioClip clip = GetSound(wordsToSay[0]);
                if(clip != null)
                    voiceSource.PlayOneShot(clip);
                wordsToSay.RemoveAt(0);
            }
        }
        /*else
        {
            if(staticSource.isPlaying)
            {
                staticSource.Stop();
                //staticSource.gameObject.SetActive(false);
            }
        }*/

        if(staticSource.isPlaying && !voiceSource.isPlaying)
            staticSource.Stop();
    }

    public void ResetWordsToSay()
    {
        wordsToSay.Clear();
    }

    public AudioClip GetSound(string soundName)
    {
        int index = -1;
        for(int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].name == soundName)
            {
                index = i;
                break;
            }
        }

        if(index <= 0) return null;

        return sounds[index].clips[Random.Range(0, sounds[index].clips.Length)];
    }

    public void SayWords(string words)
    {
        string[] subs = words.Split(' ');
        for(int i = 0; i < subs.Length; i++)
        {
            string word = subs[i].ToLower();
            if (word.IndexOf(',') >= 0)
                word = word.Substring(0, word.IndexOf(','));
            if (word.IndexOf('.') >= 0)
                word = word.Substring(0, word.IndexOf('.'));

            wordsToSay.Add(word);
        }
        /*string wordsLower = words.ToLower();
        string word = "";
        string newWords = "";
        if(words.IndexOf(' ') > 0)
        {
            word = words.Substring(0, words.IndexOf(' '));
            newWords = words.Substring(words.IndexOf(' ') + 1);
        }
        else word = words;
        if(word.IndexOf(',') >= 0)
            word = word.Substring(0, word.IndexOf(','));
        if(word.IndexOf('.') >= 0)
            word = word.Substring(0, word.IndexOf('.'));

        wordsToSay.Add(word);

        //string newWords = words.Substring(words.IndexOf(' ') + 1);
        if(newWords.Length >= 1)
        {
            SayWords(newWords);
        }*/
    }
}
