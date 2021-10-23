using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button start;

    public Button exit;
    // Start is called before the first frame update
    void Start()
    {
        start.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsConnected)
                SceneManager.LoadScene("Lobby");
            else
            {
                SceneManager.LoadScene("Loading");
            }
        });
        exit.GetComponent<Button>().onClick.AddListener(()=>{Application.Quit();});
    }
}
