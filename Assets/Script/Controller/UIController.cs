using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{

    #region Fields

    [SerializeField]
    Button btnSwitchCamera;
    [SerializeField]
    Button btnNextLevel;
    [SerializeField]
    Text txtLevel;
    [SerializeField]
    Text txtStroke;
    [SerializeField]
    Text txtStatus;
    private GameObject darkBG;

    #endregion

    #region Initialization

    void Start()
    {
        darkBG = GameObject.Find("Dark");
        StartCoroutine(FadeIn(.5f));
    }
    IEnumerator FadeIn(float delay)
    {
        yield return new WaitForSeconds(delay);
        darkBG.SetActive(false);
        GameStatus = "Your Turn";
    }
    #endregion

    #region Public Members

    public string GameStatus
    {
        get
        {
            return txtStatus.text;
        }
        set
        {
            txtStatus.text = value;
        }
    }

    public int Level
    {
        set
        {
            txtLevel.text = "Level: " + value;
        }
    }

    public int Strokes
    {
        set
        {
            txtStroke.text = "Strokes: " + value;
        }
    }


    #endregion

    #region Private Members


    #endregion
}
