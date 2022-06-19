using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAI : MonoBehaviour
{
    BlendshapeControl bsc;
    //Animator anim;
    Room room;

    float positivity = 0f;

    [SerializeField] TextMesh speechText;

    [SerializeField] string[] greetings;
    [SerializeField] string[] thanks;

    bool talking = false;

    [SerializeField] Transform replySpawnPoint;
    [SerializeField] GameObject replyPrefab;
    List<GameObject> replyBoxes = new List<GameObject>();
    List<int> replyCodes = new List<int>();
    bool processing;
    bool looking;

    public struct ReplySetup
    {
        public string label;
        public int code;
        public string terminalLabel;
    }
    public class ThingToSay
    {
        public string thing;
        public ReplySetup[] replies;
    }

    List<ThingToSay> thingsToSay = new List<ThingToSay>();


    // Start is called before the first frame update
    void Start()
    {
        bsc = GetComponent<BlendshapeControl>();
        //anim = GetComponent<Animator>();

        room = GetComponentInParent<Room>();

    }

    // Update is called once per frame
    void Update()
    {
        if(positivity > 0.9f)
            bsc.Smile();
        else if(positivity < 0.1f)
            bsc.Frown();

        if(thingsToSay.Count > 0 && !talking)
        {
            Say(thingsToSay[0].thing);
            if(thingsToSay[0].replies != null)
            {
                for(int i = 0; i < thingsToSay[0].replies.Length; i++)
                {
                    SetupReply(thingsToSay[0].replies[i].label, thingsToSay[0].replies[i].code, thingsToSay[0].replies[i].terminalLabel);
                }
            }

            thingsToSay.RemoveAt(0);
        }
    }

    void PressButton()
    {
        bsc.ReachForTransform(room.button.transform);
        room.MoveRandom();
    }

    void OnEnable()
    {
        //Debug.Log("AI Becoming Sentient");
        positivity = Random.value;
        StartCoroutine(LiveCo());

        talking = false;
        speechText.text = "";
        looking = false;
        processing = false;
    }
    void OnDisable()
    {
        StopAllCoroutines();

        ClearReplyBoxes();
    }

    void ClearReplyBoxes()
    {
        for (int i = replyBoxes.Count - 1; i >= 0; i--)
        {
            if (replyBoxes[i] != null)
                Destroy(replyBoxes[i]);
        }
        replyBoxes.Clear();
        replyCodes.Clear();
    }

    IEnumerator LiveCo()
    {
        BlendshapeControl player = null;
        bool greeted = false;
        foreach(BlendshapeControl bc in FindObjectsOfType<BlendshapeControl>())
        {
            if(bc.isPlayer)
            {
                player = bc;
                break;
            }
        }

        while(true)
        {
            //WAIT TO ROLL IN
            yield return new WaitForSeconds(Random.Range(3f, 5f));

            //FACIAL EXPRESSION AND WAVE IF HAPPY
            if(positivity > 0.6f)
            {
                bsc.Smile();
                if (!bsc.waving)
                {
                    if (Random.value + 0.5f < positivity)
                        bsc.Wave();
                    if(Random.value + 0.5f < positivity && !greeted)
                    {
                        Greet();
                        greeted = true;
                    }
                }
            }
            else if(positivity < 0.3f)
                bsc.Frown();

            if(talking)
            {
                greeted = true;
            }
            
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            //GREET
            if(Random.value > 0.5f)
            {
                if(!talking && !greeted)
                {
                    Greet();
                    greeted = true;
                    /*StartCoroutine(TalkCo(greetings[Random.Range(0, greetings.Length)]));
                    GameObject replyCube = Instantiate(replyPrefab, replySpawnPoint.position, replySpawnPoint.rotation, replySpawnPoint);
                    ReplyControl repCon = replyCube.GetComponent<ReplyControl>();
                    repCon.options = greetings;
                    replyBoxes.Add(replyCube);*/
                }
            }

            while(talking)
            {
                yield return new WaitForSeconds(Random.Range(5f, 8f));
            }

            if(Random.value > 0.5f && !looking)
            {
                StartCoroutine(LookOverCharacter(player, (int)Mathf.Sign(positivity - 0.5f)));
            }

            while(looking)
            {
                yield return new WaitForSeconds(Random.Range(2f, 4f));
            }

            //WAVE GOODBYE
            if(!bsc.waving && positivity > 0.6f)
            {
                if(Random.value < positivity)
                    bsc.Wave();
            }

            while(bsc.waving)
            {
                yield return new WaitForSeconds(Random.Range(2f, 3f));
            }

            while (talking)
            {
                yield return new WaitForSeconds(Random.Range(5f, 8f));
            }

            //LEAVE
            PressButton();

            yield return null;
        }
    }

    void Greet()
    {
        if(speechText.text != "") return;

        StartCoroutine(TalkCo(greetings[Random.Range(0, greetings.Length)]));
        GameObject replyCube = Instantiate(replyPrefab, replySpawnPoint.position, replySpawnPoint.rotation, replySpawnPoint);
        ReplyControl repCon = replyCube.GetComponent<ReplyControl>();
        repCon.options = greetings;
        replyBoxes.Add(replyCube);
        replyCodes.Add(1);
    }

    IEnumerator TalkCo(string words)
    {
        talking = true;
        bsc.talking = true;
        speechText.text = words;
        SoundLibrary.instance.SayWords(words);
        yield return new WaitForSeconds(words.Length);

        if(replyBoxes.Count > 0)
            ReceiveReply(-1);
        else
        {
            talking = false;
            bsc.talking = false;
            speechText.text = "";
        }

    }

    public void AddThingToSay(string words, ReplySetup[] replies)
    {
        ThingToSay tts = new ThingToSay();
        tts.thing = words;
        tts.replies = replies;
        thingsToSay.Add(tts);
    }
    public void AddThingToSay(string words, string replyLabel, int replyCode, string terminalLabel)
    {
        ThingToSay tts = new ThingToSay();
        tts.thing = words;
        ReplySetup[] replies = new ReplySetup[1];
        ReplySetup reply = new ReplySetup();
        reply.label = replyLabel;
        reply.code = replyCode;
        reply.terminalLabel = terminalLabel;
        replies[0] = reply;
        tts.replies = replies;
        thingsToSay.Add(tts);
        }

    public void Say(string words)
    {
        StopCoroutine("TalkCo");
        StopTalking();
        StartCoroutine(TalkCo(words));
    }

    public void StopTalking()
    {
        talking = false;
        bsc.talking = false;
        speechText.text = "";
        ClearReplyBoxes();
    }

    public void ReceiveReply(int index)
    {
        Debug.Log("Human AI received response #" + index.ToString());
        if(!processing && index >= 0)
            StartCoroutine(ProcessReply(1f, replyCodes[index]));

        ClearReplyBoxes();
        StopTalking();

        //DO RESPONSE STUFF WITH INDEX
    }

    public float GetBlendShapeWeight(int index)
    {
        return bsc.GetBlendShapeWeight(index);
    }

    IEnumerator ProcessReply(float delay, int code)
    {
        processing = true;
        yield return new WaitForSeconds(delay);
        switch(code)
        {
            case 0: //Do nothing
                break;
            case 1: //Look Over
                if(!looking)
                {
                    LookOverPlayer(1);
                }
                break;
            case 2: //this is yelp
                AddThingToSay("Yelp", null);
                break;
            case 3: //smile
                bsc.Smile();
                positivity += 0.1f;
                break;
            case 4: //frown
                bsc.Frown();
                positivity -= 0.1f;
                break;
            case 5:
                bsc.Smile();
                positivity += 0.1f;
                AddThingToSay("Thanks", null);
                if (!looking)
                {
                    LookOverPlayer(1);
                }
                break;
            case 6:
                bsc.Frown();
                positivity -= 0.1f;
                AddThingToSay("Oh, really, well", null);
                if (!looking)
                {
                    LookOverPlayer(-1);
                }
                break;
        }

        processing = false;
    }

    public bool IsTalking()
    {
        return talking;
    }

    public void SetupReply(string label, int code, string terminalString)
    {
        GameObject replyCube = Instantiate(replyPrefab, replySpawnPoint.position, replySpawnPoint.rotation, replySpawnPoint);
        ReplyControl repCon = replyCube.GetComponent<ReplyControl>();
        repCon.options = new string[1];
        repCon.options[0] = label;
        repCon.terminalString = terminalString;
        TextMesh textMesh = repCon.GetComponentInChildren<TextMesh>();
        textMesh.text = label;
        replyBoxes.Add(replyCube);
        replyCodes.Add(code);
        repCon.index = replyBoxes.Count - 1;

        if(replyBoxes.Count > 1)
        {
            for(int i = 0; i < replyBoxes.Count; i++)
            {
                replyBoxes[i].transform.localPosition += Vector3.right * (replyBoxes.Count * 0.375f);
                replyBoxes[i].transform.localPosition -= Vector3.right * 1.5f * i;
            }
        }
    }

    IEnumerator LookOverCharacter(BlendshapeControl character, int code)
    {
        Transform mainTarget = bsc.target;
        looking = true;
        Interactive[] inters = character.GetComponentsInChildren<Interactive>();

        int count = Random.Range(3, 8);

        while(looking && !talking && count > 0)
        {
            int rand = Mathf.FloorToInt(Random.value * inters.Length);

            bsc.target = inters[rand].transform;

            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

            if(code >= 0)
            {
                if(CompareBlendshapeToDesired(character, inters[rand]) > 0.7f)
                {
                    count = 0;
                    AddThingToSay("I like your " + inters[rand].displayName, "Thanks", 3, "");
                    //Say("I like your " + inters[rand].displayName);
                    //SetupReply("Thanks", 3, "");
                }
            }
            else
            {
                if(CompareBlendshapeToDesired(character, inters[rand]) < 0.3f)
                {
                    count = 0;
                    AddThingToSay("I don't like your " + inters[rand].displayName, null);
                    //Say("I don't like your " + inters[rand].displayName);
                }
            }

            count--;
        }

        bsc.target = mainTarget;
        looking = false;
    }

    void LookOverPlayer(int disposition)
    {
        if(looking) return;

        BlendshapeControl p = FindPlayer();
        if(p != null)
            StartCoroutine(LookOverCharacter(p, disposition));
    }

    BlendshapeControl FindPlayer()
    {
        foreach (BlendshapeControl bc in FindObjectsOfType<BlendshapeControl>())
        {
            if (bc.isPlayer)
            {
                return bc;
            }
        }

        return null;
    }

    float CompareBlendshapeToDesired(BlendshapeControl character, Interactive inter)
    {
        float compat = 0f;
        for(int i = 0; i < inter.options.Length; i++)
        {
            int indexMax = character.GetBlendShapeIndex(inter.options[i].blendShapeName + " Max");
            int indexMin = character.GetBlendShapeIndex(inter.options[i].blendShapeName + " Min");
            compat += Mathf.Abs(bsc.GetBlendShapeWeight(indexMax) - character.GetBlendShapeWeight(indexMax)) / (inter.options.Length * 2);
            compat += Mathf.Abs(bsc.GetBlendShapeWeight(indexMin) - character.GetBlendShapeWeight(indexMin)) / (inter.options.Length * 2);
        }

        return compat;
    }

    public BlendshapeControl GetBSC()
    {
        return bsc;
    }
}
