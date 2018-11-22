using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class AgentController : MonoBehaviour
{

    public int totalScore;
    public int teleportsCount = 2;

    private Random rand = new Random();
    private NavMeshAgent agent;
    private Alcove[] alcoves = new Alcove[10];
    private Planner planner;
    private PlanningTree currentNode;
    private WorldState worldState;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initializeAlcoves();
        initializePlanningTree();
        updateWorldState();
        planner = new Planner(currentNode, this.gameObject, agent,worldState);
        planner.execute();

    }

    void initializeAlcoves()
    {
        alcoves[0] = new Alcove(new Vector3(-60, 2, 37), false);
        alcoves[1] = new Alcove(new Vector3(-30, 2, 37), false);
        alcoves[2] = new Alcove(new Vector3(0, 2, 37), false);
        alcoves[3] = new Alcove(new Vector3(30, 2, 37), false);
        alcoves[4] = new Alcove(new Vector3(60, 2, 37), false);
        alcoves[5] = new Alcove(new Vector3(-60, 2, -37), false);
        alcoves[6] = new Alcove(new Vector3(-30, 2, -37), false);
        alcoves[7] = new Alcove(new Vector3(0, 2, -37), false);
        alcoves[8] = new Alcove(new Vector3(30, 2, -37), false);
        alcoves[9] = new Alcove(new Vector3(60, 2, -37), false);
    }
    // Update is called once per frame

    void Update()
    {
        updateAlcoves();
        updateWorldState();
        if (planner.executing == false || planner.getDistanceFromEnemy() <=25)
            planner.execute();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            totalScore++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("EnemyVision"))
        {
            this.gameObject.SetActive(false);
        }
    }

    private void updateAlcoves()
    {
        if (alcoves[0] != null)
        {
            for (int i = 0; i < 10; i++)
                alcoves[i].isEmpty = true;
        }
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Vector3 itemPos = item.transform.position;

            if (itemPos.Equals(new Vector3(-60, 2, 37)))
                alcoves[0].isEmpty = false;
            else if (itemPos.Equals(new Vector3(-30, 2, 37)))
                alcoves[1].isEmpty = false;
            else if (itemPos.Equals(new Vector3(0, 2, 37)))
                alcoves[2].isEmpty = false;
            else if (itemPos.Equals(new Vector3(30, 2, 37)))
                alcoves[3].isEmpty = false;
            else if (itemPos.Equals(new Vector3(60, 2, 37)))
                alcoves[4].isEmpty = false;
            else if (itemPos.Equals(new Vector3(-60, 2, -37)))
                alcoves[5].isEmpty = false;
            else if (itemPos.Equals(new Vector3(-30, 2, -37)))
                alcoves[6].isEmpty = false;
            else if (itemPos.Equals(new Vector3(0, 2, -37)))
                alcoves[7].isEmpty = false;
            else if (itemPos.Equals(new Vector3(30, 2, -37)))
                alcoves[8].isEmpty = false;
            else if (itemPos.Equals(new Vector3(60, 2, -37)))
                alcoves[9].isEmpty = false;

        }
    }

    public void teleport()
    {
        if (teleportsCount > 0)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            int enemyIndex;
            float enemyDistance = Vector3.Distance(enemies[0].transform.position, transform.position);
            float enemyDistance2 = Vector3.Distance(enemies[1].transform.position, transform.position);
            if (enemyDistance < enemyDistance2)
                enemyIndex = 0;
            else
            {
                enemyIndex = 1;
                enemyDistance = enemyDistance2;
            }
            float playerDistance = 1000;
            if (player !=null)
                playerDistance = Vector3.Distance(player.transform.position, transform.position);

            if (playerDistance < enemyDistance)
                teleportPlayer(player);
            else
                enemies[enemyIndex].GetComponent<EnemyController>().Respawn();

            teleportsCount--;

        }

    }

    private void teleportPlayer(GameObject player)
    {
        int playerPos = rand.Next(0, 10);
        player.transform.position = alcoves[playerPos].position;
    }

    private EnemyInfoHandler[] getEnemyInfo() {
        EnemyInfoHandler[] enemiesInfo = new EnemyInfoHandler[2];
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i=0; i < 2; i++)
        {
            enemiesInfo[i] = new EnemyInfoHandler(enemies[i].transform.position, enemies[i].GetComponent<EnemyController>().direction);
        }
        return enemiesInfo;
    }

    private void initializePlanningTree()
    {
        PlanningTree root = new PlanningTree(null, "BeAwesome");
        root.addChild(new PlanningTree(root, "Teleport"));
        root.addChild(new PlanningTree(root, "AvoidEnemy"));
        root.addChild(new PlanningTree(root, "CollectItems"));

        PlanningTree curNode = root.children[1];
        curNode.addChild(new PlanningTree(curNode, "StayInPosition"));
        curNode.addChild(new PlanningTree(curNode, "HideInAlcove"));

        currentNode = root;
    }

    private void updateWorldState()
    {
        Vector3 playerPos;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            playerPos = new Vector3(-500, 0, -500);
        else
            playerPos = player.transform.position;
        this.worldState = new WorldState(true, this.teleportsCount, this.transform.position, getEnemyInfo(),playerPos, alcoves);
        if (planner != null)
            planner.ws = this.worldState;
    }
    
}
