using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public Camera mainCamera;
    private RaycastHit raycastHit;
    private int groundMask;
    public GameObject defenseBot;
    public GameObject attackBot;
    public GameObject redGoal;
    public GameObject blueGoal;
    private const int BLUE_TEAM = -1;
    private const int RED_TEAM = 1;
    private int attackTeam;
    public GameObject ball;
    private List<GameObject> attackerList;
    private List<GameObject> defenderList;
    private int ballGottenBot;
    private int timer;
    private int timeLimit;
    private System.DateTime startTime;
    private System.DateTime botEnergyTime;
    private System.DateTime topEnergyTime;
    public Text TimeDisplay;
    public GameObject BottomTeamUI;
    public GameObject TopTeamUI;
    private int totalMatches = 5;
    private int currentMatch;
    private int totalWin = 0;
    private int totalLose = 0;
    float NextFloat(float range)
    {
        System.Random random = new System.Random();
        return (float)(random.NextDouble() * range);
    }
    void Start()
    {
        groundMask = 1 << LayerMask.NameToLayer("Ground");
        if (currentMatch == 0)
        {
            attackTeam = RED_TEAM;
        }
        currentMatch += 1;
        if (attackTeam == BLUE_TEAM)
        {
            ball.transform.position = new Vector3(NextFloat(4), 0.3f, NextFloat(8));
            BottomTeamUI.transform.Find("Text").GetComponent<Text>().text = "Player (Defender)";
            TopTeamUI.transform.Find("Text").GetComponent<Text>().text = "Enemy (Attacker)";
        }
        else if (attackTeam == RED_TEAM)
        {
            ball.transform.position = new Vector3(NextFloat(-4), 0.3f, NextFloat(-8));
            BottomTeamUI.transform.Find("Text").GetComponent<Text>().text = "Player (Attacker)";
            TopTeamUI.transform.Find("Text").GetComponent<Text>().text = "Enemy (Defender)";
        }
        ballGottenBot = -1;
        attackerList = new List<GameObject>();
        defenderList = new List<GameObject>();
        startTime = botEnergyTime = topEnergyTime = System.DateTime.Now;
        if (attkSpd == 0)
        {
            attkSpd = 1.5f * Time.deltaTime;
            carrySpd = 0.75f * Time.deltaTime;
            defSpd = 1f * Time.deltaTime;
            returnSpd = 2f * Time.deltaTime;
            ballSpd = 1.5f * Time.deltaTime;
        }
        timer = timeLimit = 140;
    }
    void FixedUpdate()
    {
        if (timer > 0)
        {
            timer = (startTime - System.DateTime.Now).Seconds + timeLimit;
        }
        else
        {
            timer = 0;
        }
        TimeDisplay.text = "" + timer;
        EnergyProcess();
        MoveAttackers();
        MoveDefenders();
        CheckAttackersActive();
        CheckTimer();
    }
    void Update()
    {
        if (Time.timeScale != 0 && Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100.0f, groundMask))
            {
                if (raycastHit.point.z > 0)
                {
                    if (attackTeam == BLUE_TEAM)
                    {
                        if (topEnergy >= 2)
                        {
                            GameObject newBot = Instantiate(attackBot, new Vector3(raycastHit.point.x, raycastHit.point.y, raycastHit.point.z), Quaternion.identity);
                            newBot.GetComponent<BotScript>().SetOrder(attackerList.Count);
                            newBot.transform.Find("Alpha_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
                            attackerList.Add(newBot);
                            UseEnergy(BLUE_TEAM, 2);
                        }
                    }
                    else if (attackTeam == RED_TEAM)
                    {
                        if (topEnergy >= 3)
                        {
                            GameObject newBot = Instantiate(defenseBot, new Vector3(raycastHit.point.x, raycastHit.point.y, raycastHit.point.z), Quaternion.identity);
                            newBot.GetComponent<DefBotScript>().SetOrder(defenderList.Count);
                            newBot.GetComponent<DefBotScript>().SetOrgPos(raycastHit.point);
                            newBot.transform.Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
                            defenderList.Add(newBot);
                            UseEnergy(BLUE_TEAM, 3);
                        }
                    }
                }
                else
                {
                    if (attackTeam == RED_TEAM)
                    {
                        if (botEnergy >= 2)
                        {
                            GameObject newBot = Instantiate(attackBot, new Vector3(raycastHit.point.x, raycastHit.point.y, raycastHit.point.z), Quaternion.identity);
                            newBot.GetComponent<BotScript>().SetOrder(attackerList.Count);
                            newBot.transform.Find("Alpha_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                            newBot.GetComponent<Animator>().SetBool("isRunning", true);
                            attackerList.Add(newBot);
                            UseEnergy(RED_TEAM, 2);
                        }
                    }
                    else if (attackTeam == BLUE_TEAM)
                    {
                        if (botEnergy >= 3)
                        {
                            GameObject newBot = Instantiate(defenseBot, new Vector3(raycastHit.point.x, raycastHit.point.y, raycastHit.point.z), Quaternion.identity);
                            newBot.GetComponent<DefBotScript>().SetOrder(defenderList.Count);
                            newBot.GetComponent<DefBotScript>().SetOrgPos(raycastHit.point);
                            newBot.transform.Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                            defenderList.Add(newBot);
                            UseEnergy(RED_TEAM, 3);
                        }
                    }
                }
            }
        }
    }
    private void CheckAttackersActive()
    {
        if (attackerList.Count > 0)
        {
            for (int i = 0; i < attackerList.Count; i++)
            {
                if (attackerList[i].GetComponent<BotScript>().getDeactiveTime() == System.DateTime.MinValue)
                {
                    return;
                }
            }
            EndMatch();
        }
    }
    private void CheckTimer()
    {
        if (timer <= 0)
        {
            EndMatch();
        }
    }
    private int botEnergy = 0;
    private int topEnergy = 0;
    public GameObject BotEnergyBarFill;
    public GameObject BotEnergyBarFull;
    public GameObject TopEnergyBarFill;
    public GameObject TopEnergyBarFull;
    private int milsecPerBar = 2000;
    private int maxEnergy = 6;
    private float fillIntervalMilsec = 100;
    private void EnergyProcess()
    {
        if (botEnergy < 6)
        {
            if ((System.DateTime.Now - botEnergyTime).Milliseconds > fillIntervalMilsec)
            {
                BotEnergyBarFill.GetComponent<Image>().fillAmount += fillIntervalMilsec / (maxEnergy * milsecPerBar);
                botEnergyTime = System.DateTime.Now;
            }
            if (BotEnergyBarFill.GetComponent<Image>().fillAmount >= (botEnergy + 1) * 1f / maxEnergy)
            {
                botEnergy += 1;
                BotEnergyBarFull.GetComponent<Image>().fillAmount += 1f / maxEnergy;
            }
        }
        if (topEnergy < 6)
        {
            if ((System.DateTime.Now - topEnergyTime).Milliseconds > fillIntervalMilsec)
            {
                TopEnergyBarFill.GetComponent<Image>().fillAmount += fillIntervalMilsec / (maxEnergy * milsecPerBar);
                topEnergyTime = System.DateTime.Now;
            }
            if (TopEnergyBarFill.GetComponent<Image>().fillAmount >= (topEnergy + 1) * 1f / maxEnergy)
            {
                topEnergy += 1;
                TopEnergyBarFull.GetComponent<Image>().fillAmount += 1f / maxEnergy;
            }
        }
    }
    private void UseEnergy(int team, int amount)
    {
        if (team == RED_TEAM)
        {
            botEnergy -= amount;
            BotEnergyBarFull.GetComponent<Image>().fillAmount -= amount * 1f / maxEnergy;
            BotEnergyBarFill.GetComponent<Image>().fillAmount -= amount * 1f / maxEnergy;
        }
        else
        {
            topEnergy -= amount;
            TopEnergyBarFull.GetComponent<Image>().fillAmount -= amount * 1f / maxEnergy;
            TopEnergyBarFill.GetComponent<Image>().fillAmount -= amount * 1f / maxEnergy;
        }
    }
    private Vector3 move;
    public GameObject topWall;
    public GameObject botWall;
    private int nearestAttacker = -2;
    public GameObject ResultUI;
    private bool attackWin = false;
    public void EndMatch()
    {
        ball.transform.parent = null;
        for (int i = 0; i < attackerList.Count; i++)
        {
            Destroy(attackerList[i]);
        }
        for (int i = 0; i < defenderList.Count; i++)
        {
            Destroy(defenderList[i]);
        }
        attackerList.Clear();
        defenderList.Clear();
        if (currentMatch < totalMatches)
        {
            if (timer <= 0)
            {
                ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "MATCH " + currentMatch + "\nDRAW";
                ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.blue;
            }
            else if (attackTeam == RED_TEAM)
            {
                if (attackWin)
                {
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "MATCH " + currentMatch + "\nWIN";
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.blue;
                    totalWin += 1;
                }
                else
                {
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "MATCH " + currentMatch + "\nLOSE";
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.red;
                    totalLose += 1;
                }
            }
            else if (attackTeam == BLUE_TEAM)
            {
                if (attackWin)
                {
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "MATCH " + currentMatch + "\nLOSE";
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.red;
                    totalLose += 1;
                }
                else
                {
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "MATCH " + currentMatch + "\nWIN";
                    ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.blue;
                    totalWin += 1;
                }
            }
        }
        else if (totalWin == totalLose)
        {
            ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "RESULT" + "\nDRAW";
            ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.blue;
        }
        else if (totalWin < totalLose)
        {
            ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "RESULT" + "\nLOSE";
            ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.red;
        }
        else
        {
            ResultUI.transform.Find("Result Text").GetComponent<Text>().text = "RESULT" + "\nWIN";
            ResultUI.transform.Find("Result Text").GetComponent<Text>().color = Color.blue;
        }
        ResultUI.SetActive(true);
        Time.timeScale = 0;
    }
    public void GoNextMatch()
    {

        ball.GetComponent<MeshRenderer>().enabled = true;
        ballGottenBot = -1;
        botEnergy = 0;
        topEnergy = 0;
        BotEnergyBarFull.GetComponent<Image>().fillAmount = 0;
        BotEnergyBarFill.GetComponent<Image>().fillAmount = 0;
        TopEnergyBarFull.GetComponent<Image>().fillAmount = 0;
        TopEnergyBarFill.GetComponent<Image>().fillAmount = 0;
        attackTeam *= -1;
        Time.timeScale = 1;
        if (currentMatch == totalMatches)
        {
            currentMatch = 0;
            totalWin = 0;
            totalLose = 0;
        }
        Start();
    }
    private float attkSpd;
    private float carrySpd;
    private float defSpd;
    private float returnSpd;
    private float ballSpd;
    void MoveAttackers()
    {
        if (ballGottenBot == -1)
        {
            if (attackerList.Count > 0)
            {
                move = new Vector3(ball.transform.position.x, 0, ball.transform.position.z);
                for (int i = 0; i < attackerList.Count; i++)
                {
                    attackerList[i].GetComponent<BotScript>().Move(move, attkSpd);
                }
                ball.transform.parent = null;
            }
            if (attackerList.Count > 1)
            {
                if (nearestAttacker == -1)
                {
                    float temp;
                    float dist = 99999999;
                    for (int i = 0; i < attackerList.Count; i++)
                    {
                        if (attackerList[i].GetComponent<BotScript>().getDeactiveTime() == System.DateTime.MinValue)
                        {
                            temp = Vector3.Distance(ball.transform.position, attackerList[i].transform.position);
                            if (temp < dist)
                            {
                                dist = temp;
                                nearestAttacker = i;
                            }
                        }
                    }
                }
                else if (nearestAttacker > -2)
                {
                    move = new Vector3(attackerList[nearestAttacker].transform.position.x, 0, attackerList[nearestAttacker].transform.position.z);
                    ball.transform.position = Vector3.MoveTowards(ball.transform.position, move, ballSpd);
                }
            }
        }
        else if (attackerList.Count > 0)
        {
            if (attackTeam == BLUE_TEAM)
            {
                move = new Vector3(redGoal.transform.position.x, 0, redGoal.transform.position.z);
            }
            else if (attackTeam == RED_TEAM)
            {
                move = new Vector3(blueGoal.transform.position.x, 0, blueGoal.transform.position.z);
            }
            attackerList[ballGottenBot].GetComponent<BotScript>().Move(move, carrySpd);
            ball.transform.parent = attackerList[ballGottenBot].transform;
            for (int i = 0; i < attackerList.Count; i++)
            {
                if (i != ballGottenBot)
                {
                    if (attackTeam == BLUE_TEAM)
                    {
                        move = new Vector3(attackerList[i].transform.position.x, 0, botWall.transform.position.z);
                    }
                    else if (attackTeam == RED_TEAM)
                    {
                        move = new Vector3(attackerList[i].transform.position.x, 0, topWall.transform.position.z);
                    }
                    attackerList[i].GetComponent<BotScript>().Move(move, attkSpd);
                }
            }
        }
    }
    void MoveDefenders()
    {
        if (defenderList.Count > 0)
        {
            for (int i = 0; i < defenderList.Count; i++)
            {
                defenderList[i].GetComponent<DefBotScript>().Move(attackerList, defSpd, returnSpd);
            }
        }
    }

    public void RemoveBot(int order)
    {
        GameObject removeBot = attackerList[order];
        attackerList.RemoveAt(order);
        Destroy(removeBot, 3);
        for (int i = order; i < attackerList.Count; i++)
        {
            attackerList[i].GetComponent<BotScript>().SetOrder(attackerList[i].GetComponent<BotScript>().GetOrder() - 1);
        }
        if (ballGottenBot > order)
        {
            ballGottenBot -= 1;
        }
    }
    public void SetBallGottenBot(int orderInList)
    {
        ballGottenBot = orderInList;
    }
    public int GetBallGotenBot()
    {
        return ballGottenBot;
    }
    public void SetNearestAttacker(int index)
    {
        nearestAttacker = index;
    }
    public List<GameObject> GetAttackerList()
    {
        return attackerList;
    }
    public List<GameObject> GetDefenderList()
    {
        return defenderList;
    }
    public void SetWinSide(bool attackWin)
    {
        this.attackWin = attackWin;
    }
}
