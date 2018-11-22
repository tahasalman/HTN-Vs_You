using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameScript : MonoBehaviour {

    // Use this for initialization
    public int playerScore = 6;
    public int agentScore = 0;
    public GameObject player;
    public GameObject agent;
    public Text playerScoreText;
    public Text agentScoreText;
    public Text playerTeleports;
    public Text agentTeleports;
    public Text gameOverScreen;
    public Text gameOverWinner;
    public Text restartInstructions;

    private List<Vector3> alcoveCoords = new List<Vector3>();

	void Start () {
        //instantiate player and agent randomly in alcoves

        alcoveCoords.Add(new Vector3(0, 0, 37));
        alcoveCoords.Add(new Vector3(0, 0, -37));
        alcoveCoords.Add(new Vector3(-30, 0, 37));
        alcoveCoords.Add(new Vector3(-30, 0, -37));
        alcoveCoords.Add(new Vector3(-60, 0, 37));
        alcoveCoords.Add(new Vector3(-60, 0, -37));
        alcoveCoords.Add(new Vector3(30, 0, 37));
        alcoveCoords.Add(new Vector3(30, 0, -37));
        alcoveCoords.Add(new Vector3(60, 0, 37));
        alcoveCoords.Add(new Vector3(60, 0, -37));

        Random rand = new Random();
        int playerPos = rand.Next(0, 10);
        int agentPos = playerPos;
        while (agentPos == playerPos)
        {
            agentPos = rand.Next(0, 10);
        }

        player = Object.Instantiate(player, alcoveCoords[playerPos],Quaternion.identity);
        agent = Object.Instantiate(agent, alcoveCoords[agentPos], Quaternion.identity);

        updateStats();
    }

    // Update is called once per frame
    void Update()
    {
        updateStats();
        if (isGameOver())
        {
            gameOver();
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

    }
    
    public void updateStats()
    {
        playerScore = player.GetComponent<PlayerController>().totalScore;
        playerScoreText.text = "Score: " + playerScore;
        agentScore = agent.GetComponent<AgentController>().totalScore;
        agentScoreText.text = "Score: " + agentScore;

        int pTeleCount = player.GetComponent<PlayerController>().teleportsCount;
        int aTeleCount = agent.GetComponent<AgentController>().teleportsCount;
        playerTeleports.text = "Teleports: " + pTeleCount;
        agentTeleports.text = "Teleports: " + aTeleCount;
    }

    public bool isGameOver()
    {
        if (playerScore + agentScore == 10)
            return true;
        if (player.activeSelf == false && agent.activeSelf == false)
            return true;
        return false;
    }

    public void gameOver()
    {
        gameOverScreen.text = "GAME OVER";
        restartInstructions.text = "'R' for Restart";
        if (playerScore > agentScore)
            gameOverWinner.text = "You Won!";
        else if (playerScore < agentScore)
            gameOverWinner.text = "You Lost!";
        else
            gameOverWinner.text = "It's a tie!";
    }
}
