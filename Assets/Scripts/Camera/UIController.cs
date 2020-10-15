using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    public GameObject[] compositionLine = new GameObject[6];

    private int switchNum = 0;
    public void NextOne()
    {
        compositionLine[switchNum++].SetActive(false);
        if (switchNum == 6)
        {
            switchNum = 0;
        }
        compositionLine[switchNum].SetActive(true);
    }

    public void LastOne()
    {
        compositionLine[switchNum--].SetActive(false);
        if (switchNum == -1)
        {
            switchNum = 5;
        }
        compositionLine[switchNum].SetActive(true);
    }
}
