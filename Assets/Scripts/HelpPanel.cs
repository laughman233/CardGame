using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour
{
    private Button close;
    void Start()
    {
        close = transform.Find("Close").GetComponent<Button>();
        close.onClick.AddListener(() => { GameController.GetInstance.helpPanel.SetActive(false);});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
