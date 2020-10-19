using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotScript : MonoBehaviour
{
    private int order;
    private System.DateTime deactiveTime;
    private BallScript ball;
    private Color originalColor;
    public void SetOrder(int order)
    {
        this.order = order;
    }

    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallScript>();
        GetComponent<Animator>().SetBool("isRunning", true);
        GetComponent<Animator>().SetFloat("runningSpd", 0.8f);
        originalColor = transform.Find("Alpha_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color;
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
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (ball.GetBallGotenBot() == -1)
            {
                other.gameObject.GetComponent<BallScript>().SetBallGottenBot(order);
                ball.GetComponent<MeshRenderer>().enabled = false;
                transform.Find("Hightlight").gameObject.SetActive(true);
                GetComponent<Animator>().SetFloat("runningSpd", 0.4f);
            }
        }
        else if (other.gameObject.tag == "Wall")
        {
            GetComponent<Animator>().SetTrigger("die");
            other.gameObject.GetComponent<WallScript>().DestroyBot(order);
        }
        else if (other.gameObject.tag == "Bot")
        {
            if (ball.GetBallGotenBot() == order && deactiveTime == System.DateTime.MinValue && other.gameObject.GetComponent<DefBotScript>().getDeactiveTime() == System.DateTime.MinValue)
            {
                deactiveTime = System.DateTime.Now;
                other.gameObject.GetComponent<DefBotScript>().setDeactiveTime(deactiveTime);
                other.gameObject.transform.Find("Aoe").gameObject.GetComponent<MeshRenderer>().enabled = false;
                transform.Find("Hightlight").gameObject.SetActive(false);
                transform.Find("Arrow").gameObject.SetActive(false);
                GetComponent<Animator>().SetBool("isRunning", false);
                transform.Find("Alpha_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.grey;
                if (ball.GetComponent<BallScript>().GetAttackerList().Count > 1)
                {
                    ball.GetComponent<BallScript>().SetBallGottenBot(-1);
                    ball.GetComponent<BallScript>().SetNearestAttacker(-1);
                    ball.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
        else if (other.gameObject.tag == "Aoe")
        {
            other.gameObject.GetComponentInParent<DefBotScript>().AddTarget(order);
        }
        else if (other.gameObject.tag == "Goal")
        {
            if (ball.GetBallGotenBot() == order)
            {
                ball.AttackWin();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Aoe")
        {
            other.gameObject.GetComponentInParent<DefBotScript>().RemoveTarget(order);
        }
    }
    public void Move(Vector3 target, float speed)
    {
        if (deactiveTime == System.DateTime.MinValue)
        {
            transform.rotation = Quaternion.LookRotation(target - transform.position);
            transform.position = Vector3.MoveTowards(transform.position, target, speed);
        }
        else if ((System.DateTime.Now - deactiveTime).Seconds >= 4)
        {
            deactiveTime = System.DateTime.MinValue;
            transform.Find("Arrow").gameObject.SetActive(true);
            GetComponent<Animator>().SetBool("isRunning", true);
            GetComponent<Animator>().SetFloat("runningSpd", 0.8f);
            transform.Find("Alpha_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = originalColor;
        }
    }
}
