using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*[System.Serializable]
public class BlendParams
{
    public 
}*/

public class BlendshapeControl : MonoBehaviour
{
    public bool isPlayer = false;
    Camera cam;
    Vector3 camPosInit = Vector3.zero;

    public bool random = false;

    float[] characterSetup;
    float[] desiredSetup;

    [SerializeField] int smileLeft;
    [SerializeField] int frownLeft;
    [SerializeField] int smileRight;
    [SerializeField] int frownRight;
    [SerializeField] int eyeOpenLeft;
    [SerializeField] int eyeCloseLeft;
    [SerializeField] int eyeOpenRight;
    [SerializeField] int eyeCloseRight;
    [SerializeField] int browLeftUp;
    [SerializeField] int browLeftDown;
    [SerializeField] int browRightUp;
    [SerializeField] int browRightDown;
    [SerializeField] int mouthOpen;
    [SerializeField] int mouthClose;

    Animator animator;
    SkinnedMeshRenderer rend;
    List<Material> materials = new List<Material>();

    public Color leftEyeColor;
    public Color rightEyeColor;
    public Color skinColor;

    public bool talking = false;
    bool isTalking = false;
    public float talkMult = 2f;

    public bool waving = false;
    bool smiling = false;
    bool frowning = false;
    public enum Eyebrow{ Preset, Left, Right, Up, Down}
    public Eyebrow eyebrowState = Eyebrow.Preset;

    int turnState = 0;

    public Transform head;

    public float reachMult = 2;

    public Transform target;
    Transform reachTarget;
    Transform mainTarget;

    InteractControl interactControl;

    Interactive[] interactives;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        rend.GetMaterials(materials);
        Randomize();

        smileLeft = rend.sharedMesh.GetBlendShapeIndex("SmileLeft Max");
        frownLeft = rend.sharedMesh.GetBlendShapeIndex("SmileLeft Min");
        smileRight = rend.sharedMesh.GetBlendShapeIndex("SmileRight Max");
        frownRight = rend.sharedMesh.GetBlendShapeIndex("SmileRight Min");
        eyeOpenLeft = rend.sharedMesh.GetBlendShapeIndex("EyeOpenLeft Max");
        eyeCloseLeft = rend.sharedMesh.GetBlendShapeIndex("EyeOpenLeft Min");
        eyeOpenRight = rend.sharedMesh.GetBlendShapeIndex("EyeOpenRight Max");
        eyeCloseRight = rend.sharedMesh.GetBlendShapeIndex("EyeOpenRight Min");
        browLeftUp = rend.sharedMesh.GetBlendShapeIndex("BrowRaiseLeft Max");
        browLeftDown = rend.sharedMesh.GetBlendShapeIndex("BrowRaiseLeft Min");
        browRightUp = rend.sharedMesh.GetBlendShapeIndex("BrowRaiseRight Max");
        browRightDown = rend.sharedMesh.GetBlendShapeIndex("BrowRaiseRight Min");
        mouthOpen = rend.sharedMesh.GetBlendShapeIndex("MouthOpen Max");
        mouthClose = rend.sharedMesh.GetBlendShapeIndex("MouthOpen Min");

        StartCoroutine(Blink(eyeOpenLeft, eyeCloseLeft));
        StartCoroutine(Blink(eyeOpenRight, eyeCloseRight));
        StartCoroutine(EyebrowShift());
    }

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        if(cam != null)
            camPosInit = cam.transform.localPosition;

        interactControl = GetComponentInChildren<InteractControl>();

        interactives = GetComponentsInChildren<Interactive>();
        if(isPlayer)
        {
            foreach(Interactive inter in interactives)
            {
                inter.GetComponent<Collider>().enabled = false;
            }
        }
    }

    IEnumerator Blink(int openIndex, int closeIndex)
    {
        bool closing = false;
        while(true)
        {
            if(closing)
            {
                rend.SetBlendShapeWeight(openIndex, 0f);
                rend.SetBlendShapeWeight(closeIndex, 1f);
                closing = false;
                yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
            }
            else
            {
                rend.SetBlendShapeWeight(openIndex, characterSetup[openIndex]);
                rend.SetBlendShapeWeight(closeIndex, characterSetup[closeIndex]);
                closing = true;
                yield return new WaitForSeconds(Random.Range(3f, 10f));
            }
        }
    }
    IEnumerator EyebrowShift()
    {
        while(true)
        {
            eyebrowState = (Eyebrow)Random.Range(0, 5);
            SetEyebrows();
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    IEnumerator Talk()
    {
        isTalking = true;
        while(talking)
        {
            rend.SetBlendShapeWeight(mouthOpen, Mathf.PerlinNoise(Time.time * talkMult, Time.time * talkMult) * 100);
            rend.SetBlendShapeWeight(mouthClose, Mathf.PerlinNoise(Time.time * 2 * talkMult, Time.time * 2 * talkMult) * 100);
            yield return new WaitForFixedUpdate();
        }

        rend.SetBlendShapeWeight(mouthOpen, characterSetup[mouthOpen]);
        rend.SetBlendShapeWeight(mouthClose, characterSetup[mouthClose]);

        isTalking = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(talking)
        {
            if(!isTalking)
                StartCoroutine(Talk());
        }

        animator.SetBool("waving", waving);

        if(isPlayer)
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            animator.SetBool("crouching", Input.GetButton("Fire2"));
        }
    }

    public void Wave()
    {
        if(!waving)
            StartCoroutine(WaveCo());
    }
    IEnumerator WaveCo()
    {
        waving = true;
        while(waving)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 5f));

            waving = false;
        }
    }

    public void Turn(int direction)
    {
        if(animator.GetBool("turning")) return;

        animator.SetBool("turning", true);
        animator.SetFloat("turnDirection", direction);
        turnState++;
        turnState %= 4;
        //Invoke("ToggleOffTurning", 0.5f);
    }
    public void ToggleOffTurning()
    {
        animator.SetBool("turning", false);
    }

    void SetEyebrows()
    {
        switch (eyebrowState)
        {
            case Eyebrow.Preset:
                rend.SetBlendShapeWeight(browLeftUp, characterSetup[browLeftUp]);
                rend.SetBlendShapeWeight(browLeftDown, characterSetup[browLeftDown]);
                rend.SetBlendShapeWeight(browRightUp, characterSetup[browRightUp]);
                rend.SetBlendShapeWeight(browRightDown, characterSetup[browRightDown]);
                break;
            case Eyebrow.Left:
                rend.SetBlendShapeWeight(browLeftUp, 100);
                rend.SetBlendShapeWeight(browLeftDown, 0);
                rend.SetBlendShapeWeight(browRightUp, 0);
                rend.SetBlendShapeWeight(browRightDown, 100);
                break;
            case Eyebrow.Right:
                rend.SetBlendShapeWeight(browLeftUp, 0);
                rend.SetBlendShapeWeight(browLeftDown, 100);
                rend.SetBlendShapeWeight(browRightUp, 100);
                rend.SetBlendShapeWeight(browRightDown, 0);
                break;
            case Eyebrow.Up:
                rend.SetBlendShapeWeight(browLeftUp, 100);
                rend.SetBlendShapeWeight(browLeftDown, 0);
                rend.SetBlendShapeWeight(browRightUp, 100);
                rend.SetBlendShapeWeight(browRightDown, 0);
                break;
            case Eyebrow.Down:
                rend.SetBlendShapeWeight(browLeftUp, 0);
                rend.SetBlendShapeWeight(browLeftDown, 100);
                rend.SetBlendShapeWeight(browRightUp, 0);
                rend.SetBlendShapeWeight(browRightDown, 100);
                break;
        }
    }

    void OnValidate()
    {
        if (random)
        {
            random = false;
            Randomize();
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if(!isPlayer) 
        {
            if(target != null && turnState == 0)
            {
                animator.SetLookAtPosition(target.position);
                animator.SetLookAtWeight(1);
            }

            if(reachTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, reachTarget.position);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

                //float angleX = point ? 0 : -90;
                //float angleZ = point ? -90 : 0;
                Quaternion rot = head.rotation * Quaternion.Euler(-90, 0, 0);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rot);//Quaternion.Slerp())
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            }

            return;
        }

        Vector2 mousePos = Input.mousePosition;
        mousePos.x -= Screen.width / 2;
        mousePos.x /= Screen.width;
        mousePos.y -= Screen.height / 2;
        mousePos.y /= Screen.height;
        Vector3 lookDir = head.TransformDirection(new Vector3(mousePos.x, mousePos.y, 1));
        animator.SetLookAtPosition(head.position + lookDir);
        animator.SetLookAtWeight(1);

        if(Input.GetButton("Fire1"))
        {
            Vector3 pos = head.position + head.forward;
            bool point = false;
            if(interactControl != null)
            {
                pos = interactControl.interactPoint;
                if(interactControl.currentInteractive != null)
                    point = interactControl.currentInteractive.pointAtMe;
            }
            else pos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, reachMult));
            //float angleX = Input.mousePosition.y > Screen.height / 3 ? -90 : 0;
            //float angleZ = Input.mousePosition.y < Screen.height / 3 ? -90 : 0;
            float angleX = point ? 0 : -90;
            float angleZ = point ? -90 : 0;
            Quaternion rot = head.rotation * Quaternion.Euler(angleX, 0, angleZ);
            animator.SetIKPosition(AvatarIKGoal.RightHand, pos);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rot);//Quaternion.Slerp())
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        }
    }

    public void Randomize()
    {
        if(rend == null)
            rend = GetComponentInChildren<SkinnedMeshRenderer>();

        characterSetup = new float[rend.sharedMesh.blendShapeCount];
        desiredSetup = new float[rend.sharedMesh.blendShapeCount];

        float rand = 0f;
        for (int i = 0; i < rend.sharedMesh.blendShapeCount; i++)
        {
            rand = Random.value * 100;
            rend.SetBlendShapeWeight(i, rand);
            characterSetup[i] = rand;
            if(isPlayer) desiredSetup[i] = 0;
            else desiredSetup[i] = Random.value * 100;
        }

        if(Application.IsPlaying(gameObject))
        {
            leftEyeColor = new Color(Random.value, Random.value, Random.value);
            materials[1].color = leftEyeColor;
            rightEyeColor = new Color(Random.value, Random.value, Random.value);
            materials[2].color = rightEyeColor;
            skinColor = new Color(Random.value, Random.value, Random.value);
            materials[0].color = skinColor;
        }

        /*characterSetup = new float[rend.sharedMesh.blendShapeCount];
        for(int i = 0; i < characterSetup.Length; i++)
        {
            characterSetup[i] = rend.GetBlendShapeWeight(i);
        }*/

        if(cam != null)
        {
            cam.transform.localPosition = camPosInit + Vector3.forward * (rend.GetBlendShapeWeight(rend.sharedMesh.GetBlendShapeIndex("HeadSize Max")) / 100) * 0.5f;
            cam.transform.localPosition -= Vector3.forward * (rend.GetBlendShapeWeight(rend.sharedMesh.GetBlendShapeIndex("HeadSize Min")) / 100) * 0.5f;
        }

        animator.SetFloat("cycleOffset", Random.value);
        animator.SetFloat("speed", Random.Range(0.5f, 1.5f));

    }

    public void ReachForTransform(Transform tran)
    {
        reachTarget = tran;
    }

    void OnEnable()
    {
        reachTarget = null;
        waving = false;
    }

    public void Smile()
    {
        if(!smiling)
            StartCoroutine(SmileCo());
    }

    IEnumerator SmileCo()
    {
        smiling = true;
        rend.SetBlendShapeWeight(smileLeft, 100);
        rend.SetBlendShapeWeight(frownLeft, 0);
        rend.SetBlendShapeWeight(smileRight, 100);
        rend.SetBlendShapeWeight(frownRight, 0);
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        rend.SetBlendShapeWeight(smileLeft, characterSetup[smileLeft]);
        rend.SetBlendShapeWeight(frownLeft, characterSetup[frownLeft]);
        rend.SetBlendShapeWeight(smileRight, characterSetup[smileRight]);
        rend.SetBlendShapeWeight(frownRight, characterSetup[frownRight]);

        smiling = false;
    }

    public void Frown()
    {
        if(!frowning)
            StartCoroutine(FrownCo());
    }
    IEnumerator FrownCo()
    {
        frowning = true;

        rend.SetBlendShapeWeight(smileLeft, 0);
        rend.SetBlendShapeWeight(frownLeft, 100);
        rend.SetBlendShapeWeight(smileRight, 0);
        rend.SetBlendShapeWeight(frownRight, 100);

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        rend.SetBlendShapeWeight(smileLeft, characterSetup[smileLeft]);
        rend.SetBlendShapeWeight(frownLeft, characterSetup[frownLeft]);
        rend.SetBlendShapeWeight(smileRight, characterSetup[smileRight]);
        rend.SetBlendShapeWeight(frownRight, characterSetup[frownRight]);

        frowning = false;
    }

    public float[] GetSetup()
    {
        return characterSetup;
    }
    public float CheckCompatibility(float[] suitorWeights)
    {
        if(suitorWeights.Length > desiredSetup.Length)
        {
            Debug.LogError("Suitor has too many features to compare, apparently");
            return 0f;
        }

        float delta = 0f;
        for(int i = 0; i < suitorWeights.Length; i++)
        {
            delta += Mathf.Abs(desiredSetup[i] - suitorWeights[i]) / suitorWeights.Length;
        }
        return delta / 100;
    }

    public float GetBlendShapeWeight(int index)
    {
        return characterSetup[index];
    }
    public float GetBlendShapeWeight(string shapeName)
    {
        return GetBlendShapeWeight(GetBlendShapeIndex(shapeName));
    }

    public int GetBlendShapeIndex(string shapeName)
    {
        return rend.sharedMesh.GetBlendShapeIndex(shapeName);
    }

    public void AdjustDesired(int index, float value, int direction)
    {
        float thisValue = desiredSetup[index];

        if(thisValue <= Mathf.Epsilon && direction >= 0)
        {
            desiredSetup[index] = value;
        }
    }
}
