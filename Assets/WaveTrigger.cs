using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    bool tracking;

    Interactive interactive;

    List<Vector3> positions = new List<Vector3>();

    BlendshapeControl guy;

    // Start is called before the first frame update
    void Start()
    {
        interactive = GetComponent<Interactive>();

        guy = GetComponentInParent<Room>().GetComponentInChildren<BlendshapeControl>();

        if(guy == null)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrackWave()
    {
        if(!tracking && guy != null)
        {
            StartCoroutine(TrackCo());
            //Debug.Log("Should Be Tracking Wave");
        }
    }

    public void StopTracking()
    {
        StopAllCoroutines();
        tracking = false;
    }

    IEnumerator TrackCo()
    {
        tracking = true;
        positions.Clear();
        float velMag = 0;
        while(tracking && interactive.interactor != null)
        {
            Vector3 newPos = interactive.interactor.interactPoint;
            //if(newPos == Vector3.zero)
            //    tracking = false;
            //else
            //if(newPos != Vector3.zero)
            //{
                positions.Add(newPos);

                Vector3 velocity = Vector3.zero;
                if(positions.Count > 1)
                {
                    velocity = newPos - positions[positions.Count - 2];
                    velocity /= Time.deltaTime;
                }
                velMag += velocity.magnitude;

                if(positions.Count > 60 && velMag / positions.Count > 1f)
                {
                    guy.Wave();
                }

                yield return new WaitForFixedUpdate();
            //}
        }

        tracking = false;
    }
}
