using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ground;
    public void SetBallGottenBot(int index)
    {
        ground.GetComponent<GameScript>().SetBallGottenBot(index);
    }
    public int GetBallGotenBot()
    {
        return ground.GetComponent<GameScript>().GetBallGotenBot();
    }
    public void SetNearestAttacker(int index)
    {
        ground.GetComponent<GameScript>().SetNearestAttacker(index);
    }
    public List<GameObject> GetAttackerList()
    {
        return ground.GetComponent<GameScript>().GetAttackerList();
    }
    public List<GameObject> GetDefenderList()
    {
        return ground.GetComponent<GameScript>().GetDefenderList();
    }
    public void AttackWin()
    {
        ground.GetComponent<GameScript>().SetWinSide(true);
        ground.GetComponent<GameScript>().EndMatch();
    }
}
