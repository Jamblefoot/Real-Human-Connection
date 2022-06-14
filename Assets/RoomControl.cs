using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomControl : MonoBehaviour
{

    [SerializeField] GameObject roomEmptyPrefab;
    [SerializeField] GameObject roomEmptyLitPrefab;
    [SerializeField] GameObject roomLitWithGuyPrefab;

    [SerializeField] MeshRenderer window;

    public class RoomSetup
    {
        public Transform tran;
        public bool lit;
        public bool active;
    }
    RoomSetup[] rooms;

    bool moving;

    float roomWidth = 10f;
    float roomHeight = 7.5f;
    float moveSpeed = 5f;

    int currentRoom = 0;
    int nextRoom = -1;
    // Start is called before the first frame update
    void Start()
    {
        rooms = new RoomSetup[6];
        for(int i = 0; i < 6; i++)
        {
            rooms[i] = new RoomSetup();
            GameObject room = null;
            if(i < 2)
            {
                room = Instantiate(roomLitWithGuyPrefab, transform.position + Vector3.right * 50, transform.rotation, transform);
                rooms[i].lit = true;
            }
            else if(i < 4)
            {
                room = Instantiate(roomEmptyLitPrefab, transform.position + Vector3.right * 50, transform.rotation, transform);
                rooms[i].lit = true;
            }
            else
            {
                room = Instantiate(roomEmptyPrefab, transform.position + Vector3.right * 50, transform.rotation, transform);
                rooms[i].lit = false;
            }
            rooms[i].tran = room.transform;
            rooms[i].active = false;
            rooms[i].tran.gameObject.SetActive(false);
        }

        rooms[5].active = true;
        rooms[5].tran.gameObject.SetActive(true);
        rooms[5].tran.localPosition = Vector3.zero;
        currentRoom = 5;
        Move(Random.Range(1, 6), Random.value > 0.5f, Random.value > 0.5f ? 1 : -1);
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        if(!moving)
        {
            if(Mathf.Abs(vertical) > 0.5f)
            {
                Move(Random.Range(1, 6), false, -1 * Mathf.RoundToInt(Mathf.Sign(vertical)));
            }
            else if(Mathf.Abs(horizontal) > 0.5f)
                Move(Random.Range(1, 6), true, Mathf.RoundToInt(Mathf.Sign(horizontal)));
        }

    }

    public void Move(int amount, bool xAxis, int direction)
    {
        if(!moving)
            StartCoroutine(MoveCo(amount, xAxis, direction));
    }

    IEnumerator MoveCo(int amount, bool xAxis, int direction)
    {
        moving = true;
        window.material.SetFloat("_Glossiness", 1f);
        int counter = amount;

        while(counter > 0)
        {
            if(nextRoom < 0)
            {
                if(counter == 1)
                {
                    if(currentRoom == 0)
                        nextRoom = 1;
                    else nextRoom = 0;
                }
                //else if(counter == 2)
                //    nextRoom = SelectRoom(1);
                else nextRoom = SelectRoom();

                if(nextRoom < 2)
                {
                    BlendshapeControl bsc = rooms[nextRoom].tran.GetComponentInChildren<BlendshapeControl>();
                    if(bsc != null)
                        bsc.Randomize();
                }
                rooms[nextRoom].active = true;
                rooms[nextRoom].tran.gameObject.SetActive(true);
                Vector3 pos = rooms[currentRoom].tran.position;
                if(xAxis)
                    pos -= direction * transform.right * roomWidth;
                else pos -= direction * transform.up * roomHeight;
                

                rooms[nextRoom].tran.position = pos;
            }

            Vector3 move = Vector3.zero;
            if(xAxis) //horizontal move
            {

                move = direction * transform.right * Time.deltaTime * moveSpeed;
            }
            else move = direction * transform.up * Time.deltaTime * moveSpeed;

            rooms[currentRoom].tran.position += move;
            
            if(nextRoom >= 0)
                rooms[nextRoom].tran.position += move;

            if(Mathf.Abs(rooms[currentRoom].tran.position.x) > roomWidth - 0.1f || Mathf.Abs(rooms[currentRoom].tran.position.y) > roomHeight - 0.1f)
            {
                rooms[currentRoom].tran.position += Vector3.right * 50;
                rooms[currentRoom].active = false;
                rooms[currentRoom].tran.gameObject.SetActive(false);
                currentRoom = nextRoom;
                nextRoom = -1;
                counter--;
            }


            yield return new WaitForFixedUpdate();
        }

        window.material.SetFloat("_Glossiness", 0.8f);
        //NEED CLUNK SOUND
        moving = false;
    }

    int SelectRoom(int minIndex = 0)
    {
        int rand = Random.Range(minIndex, rooms.Length);
        if(rand < 0 || rand >= rooms.Length)
            return SelectRoom(minIndex);
        if(!rooms[rand].active)
            return rand;
        else return SelectRoomDescending();//SelectRoom(minIndex);
    }

    int SelectRoomDescending()
    {
        for(int i = rooms.Length - 1; i >= 0; i--)
        {
            if(!rooms[i].active)
            {
                return i;
            }
        }

        return 4;
    }
}
