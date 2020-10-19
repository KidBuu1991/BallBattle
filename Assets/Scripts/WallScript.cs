using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    public GameObject Ground;
    public void DestroyBot(int index)
    {
        Ground.GetComponent<GameScript>().RemoveBot(index);
    }   
}
