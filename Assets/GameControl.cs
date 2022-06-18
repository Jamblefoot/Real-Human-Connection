using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    public bool inMenu = false;

    [SerializeField] Canvas menuCanvas;
    // Start is called before the first frame update
    void Start()
    {
        if(GameControl.instance == null)
            GameControl.instance = this;
        else DestroyImmediate(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            inMenu = !inMenu;
        }

        if(inMenu)
        {
            if(!menuCanvas.gameObject.activeSelf)
                menuCanvas.gameObject.SetActive(true);
        }
        else
        {
            if(menuCanvas.gameObject.activeSelf)
                menuCanvas.gameObject.SetActive(false);
        }
    }

    public void SetMenu(bool setting)
    {
        if(inMenu != setting)
        {

        }

        inMenu = setting;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
