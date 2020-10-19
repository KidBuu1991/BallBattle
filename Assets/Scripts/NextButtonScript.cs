using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButtonScript : MonoBehaviour
{
    public GameObject game;
    public GameObject ResultUI;
    public void NextMatch()
    {
        ResultUI.SetActive(false);
        game.GetComponent<GameScript>().GoNextMatch();
    }
}
