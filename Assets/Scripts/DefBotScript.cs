using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefBotScript : MonoBehaviour
{
    private int order;
    private System.DateTime deactiveTime;
    private List<int> targetList;
    private int target = -1;
    private Vector3 orgPos;
    private BallScript ball;
    private Color originalColor;
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallScript>();
        targetList = new List<int>();
        GetComponent<Animator>().SetBool("isRunning", false);
        GetComponent<Animator>().SetFloat("runningSpd", 0.6f);
        originalColor = transform.Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color;
    }
    void FixedUpdate()
    {
        if (deactiveTime == System.DateTime.MinValue)
        {
            Vector3 ballVec = new Vector3(ball.transform.position.x, 0, ball.transform.position.z);
            transform.rotation = Quaternion.LookRotation(ballVec - transform.position);
        }
        else
        {
            transform.Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.grey;
            if (transform.position == orgPos)
            {
                GetComponent<Animator>().SetBool("isRunning", false);
            }
        }
    }
    public void SetOrder(int order)
    {
        this.order = order;
    }

    public int GetOrder()
    {
        return order;
    }
    public void setDeactiveTime(System.DateTime time)
    {
        deactiveTime = time;
    }
    public System.DateTime getDeactiveTime()
    {
        return deactiveTime;
    }
    public void AddTarget(int target)
    {
        targetList.Add(target);
    }
    public void RemoveTarget(int target)
    {
        targetList.Remove(target);
    }
    public void SetOrgPos(Vector3 pos)
    {
        orgPos = pos;
    }
    private Vector3 targetPos;
    public void Move(List<GameObject> attackerList, float moveSpd, float returnSpd)
    {
        if (deactiveTime == System.DateTime.MinValue)
        {
            target = -1;
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] == ball.GetBallGotenBot())
                {
                    target = targetList[i];
                    break;
                }
            }
            if (target > -1)
            {
                if (attackerList[target].GetComponent<BotScript>().getDeactiveTime() == System.DateTime.MinValue)
                {
                    GetComponent<Animator>().SetBool("isRunning", true);
                    targetPos = new Vector3(attackerList[target].transform.position.x, attackerList[target].transform.position.y, attackerList[target].transform.position.z);
                    transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.05f);
                }
                else if (transform.position != orgPos)
                {
                    GetComponent<Animator>().SetBool("isRunning", true);
                    transform.rotation = Quaternion.LookRotation(orgPos - transform.position);
                    transform.position = Vector3.MoveTowards(transform.position, orgPos, 0.1f);
                }
            }
            else if (transform.position != orgPos)
            {
                GetComponent<Animator>().SetBool("isRunning", true);
                transform.rotation = Quaternion.LookRotation(orgPos - transform.position);
                transform.position = Vector3.MoveTowards(transform.position, orgPos, 0.1f);
            }
        }
        else
        {
            if (transform.position != orgPos)
            {
                GetComponent<Animator>().SetBool("isRunning", true);
                transform.rotation = Quaternion.LookRotation(orgPos - transform.position);
                transform.position = Vector3.MoveTowards(transform.position, orgPos, 0.1f);
            }
            if ((System.DateTime.Now - deactiveTime).Seconds >= 4)
            {
                transform.Find("Aoe").gameObject.GetComponent<MeshRenderer>().enabled = true;
                deactiveTime = System.DateTime.MinValue;
                transform.Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = originalColor;
            }
        }
    }
}
