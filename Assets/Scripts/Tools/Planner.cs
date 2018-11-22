using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Alcove
{
    public Vector3 position { get; set; }
    public bool isEmpty { get; set; }

    public Alcove(Vector3 position, bool isEmpty)
    {
        this.position = position;
        this.isEmpty = isEmpty;
    }

}

public class EnemyInfoHandler
{
    public Vector3 position;
    public int direction;

    public EnemyInfoHandler(Vector3 position, int direction)
    {
        this.position = position;
        this.direction = direction;
    }
}


public class WorldState
{
    public bool itemsLeft;
    public int teleportsLeft;
    public Vector3 agentPosition;
    public EnemyInfoHandler[] enemyInfo = new EnemyInfoHandler[2];
    public Vector3 playerPosition;
    public Alcove[] alcoves;

    public WorldState(bool itemsLeft,
        int teleportsLeft,
        Vector3 agentPosition,
        EnemyInfoHandler[] enemyInfo,
        Vector3 playerPosition,
        Alcove [] alcoves)
    {
        this.itemsLeft = itemsLeft;
        this.teleportsLeft = teleportsLeft;
        this.agentPosition = agentPosition;
        this.enemyInfo = enemyInfo;
        this.playerPosition = playerPosition;
        this.alcoves = alcoves;
    }
}


public class PlanningTree
{
    public PlanningTree parent;
    public List<PlanningTree> children = new List<PlanningTree>();
    public string task;

    public PlanningTree(PlanningTree parent, string task)
    {
        this.parent = parent;
        this.task = task;
    }

    public void addChildToPos(int childPos, PlanningTree node)
    {
        children.Insert(childPos, node);
    }

    public void addChild(PlanningTree node)
    {
        children.Add(node);
    }

}

public class Planner
{

    public bool executing = false;
    public PlanningTree root;
    public GameObject agent;
    public NavMeshAgent agentNav;
    public WorldState ws;

    public Planner(PlanningTree root, GameObject agent, NavMeshAgent agentNav, WorldState ws)
    {
        this.root = root;
        this.agent = agent;
        this.agentNav = agentNav;
        this.ws = ws;

    }

    public void execute()
    {
        executing = true;
        runPlanFromNode(root);
        executing = false;
    }

    private int runPlanFromNode(PlanningTree node)
    {
       // Debug.Log(node.task);
        if (!preconditionsMet(node.task))
            return -1;
        int result = -1;
        if (node.children.Count == 0)
            return taskExecutor(node.task);

        for (int i = 0; i < node.children.Count; i++)
        {
            result = runPlanFromNode(node.children[i]);
            if (result == 1)
            {
                executing = false;
                return 1;
            }
        }
        return result;
    }

    private int taskExecutor(string task)
    {
        int result = 1;
        if (task.Equals("Teleport"))
        {
            if (preconditionsMet(task))
                teleport();
            else
                return -1;

        }
        else if (task.Equals("StayInPosition"))
        {
            if (preconditionsMet(task))
                stayInPosition();
            else
                return -1;
        }
        else if (task.Equals("CollectItems")){
            if (preconditionsMet(task))
                return collectNearestItem();
        }
        else if (task.Equals("AvoidEnemy"))
        {
            if (preconditionsMet(task))
                return 1;
            else
                return -1;
        }
        else
        {
            int index = findNearestAlcove();
            collectItem(index);
        }

        return result;
    }

    private bool preconditionsMet(string task)
    {
 
        if (task.Equals("Teleport"))
        {
            return (ws.teleportsLeft > 0 && 
                (getDistanceFromPlayer() <=20 || getDistanceFromEnemy() <= 20) && 
                !(ws.agentPosition.z >= 35 || ws.agentPosition.z <= -35));
        }
        else if (task.Equals("AvoidEnemy"))
        {
            return (getDistanceFromEnemy() <= 40);
        }
        else if (task.Equals("CollectItems"))
        {
            return true;
        }
        else if (task.Equals("StayInPosition")) {
            return (ws.agentPosition.z >= 35 || ws.agentPosition.z <= -35 || (ws.agentPosition.z < 5 && ws.agentPosition.z > -5));
        }
        else if (task.Equals("HideInAlcove")) { }
           
        return true;
    }


    
    void stayInPosition()
    {
        Vector3 currentPos = ws.agentPosition;
        agentNav.destination = currentPos;
    }

    void teleport()
    {
        agent.GetComponent<AgentController>().teleport();
    }


    void collectItem(int itemIndex)     // Collect Item at the specified Index
    {
        agentNav.destination = ws.alcoves[itemIndex].position;
    }

    int getNearestItemInRow(int start, int stop)
    {
        int itemIndex = -1;
        float shortestDistance;
        Vector3 currentPos = agent.transform.position;

        int curIndex = start;
        while (curIndex < stop && ws.alcoves[curIndex].isEmpty)
            curIndex++;
        if (curIndex < stop && !ws.alcoves[curIndex].isEmpty)         // if atleast one item in this side of level
        {
            itemIndex = curIndex;
            shortestDistance = System.Math.Abs(currentPos.x - ws.alcoves[itemIndex].position.x);
            curIndex++;

            float curDistance;
            while (curIndex < stop)
            {
                if (ws.alcoves[curIndex].isEmpty)
                {
                    curIndex++;
                    continue;
                }
                curDistance = System.Math.Abs(currentPos.x - ws.alcoves[curIndex].position.x);
                if (curDistance < shortestDistance)
                {
                    itemIndex = curIndex;
                    shortestDistance = curIndex;
                }
                curIndex++;
            }
        }



        return itemIndex;
    }



    int getNearestSafeAlcove(int start, int stop)
    {
        int index=start;
        int curIndex = start;
        float shortestDistance = System.Math.Abs(ws.agentPosition.x - ws.alcoves[index].position.x);
        curIndex++;

        float curDistance;
        while (curIndex < stop)
        {
            curDistance = System.Math.Abs(ws.agentPosition.x - ws.alcoves[curIndex].position.x);
            if (curDistance < shortestDistance)
            {
                index = curIndex;
                shortestDistance = curIndex;
            }
            curIndex++;
        }

        return index;
    }
        public float getDistanceFromEnemy()
    {
        float enemyDistance = Vector3.Distance(ws.enemyInfo[0].position, ws.agentPosition);
        float enemyDistance2 = Vector3.Distance(ws.enemyInfo[1].position, ws.agentPosition);
        if (enemyDistance > enemyDistance2)
            return enemyDistance2;
        else
            return enemyDistance;
    }

    float getDistanceFromPlayer()
    {
        return Vector3.Distance(ws.playerPosition, ws.agentPosition);
    }

    int findNearestItem()
    {
        Vector3 currentPos = agent.transform.position;
        int itemIndex = -1;
        if (currentPos.z > 0)
        {
            itemIndex = getNearestItemInRow(0, 5);
            if (itemIndex == -1)
            {
                itemIndex = getNearestItemInRow(5, 10);
            }
        }
        else
        {
            itemIndex = getNearestItemInRow(5, 10);
            if (itemIndex == -1)
                itemIndex = getNearestItemInRow(0, 5);
        }

        return itemIndex;

    }

    int collectNearestItem()
    {
        int itemIndex = findNearestItem();
        if (itemIndex != -1)
            collectItem(itemIndex);
        else
            return -1;
        return 1;
    }

    int findNearestAlcove()
    {
        
        EnemyInfoHandler nearestEnemy;
        if(ws.agentPosition.z > 0)
        {
            if (ws.enemyInfo[0].position.z > 0)
                nearestEnemy = ws.enemyInfo[0];
            else
                nearestEnemy = ws.enemyInfo[1];
        }
        else
        {
            if (ws.enemyInfo[0].position.z < 0)
                nearestEnemy = ws.enemyInfo[0];
            else
                nearestEnemy = ws.enemyInfo[1];
        }
        if (ws.agentPosition.z > 0)
        {
            if (nearestEnemy.position.x <= 0)
                return getNearestSafeAlcove(3, 5);
            else
                return getNearestSafeAlcove(0, 3);
        }
        else
        {
            if (nearestEnemy.position.x <= 0)
                return getNearestSafeAlcove(8, 10);
            else
                return getNearestSafeAlcove(5, 8);
        }
        
    }


}


