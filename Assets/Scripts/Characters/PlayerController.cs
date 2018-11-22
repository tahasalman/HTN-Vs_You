using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class PlayerController : MonoBehaviour {

	// Use this for initialization
    public int speed = 20;
    public int teleportsCount = 2;
    public int totalScore = 0;

    private List<Vector3> alcoveCoords = new List<Vector3>();
    private Random rand;
    void Start () {
        rand = new Random();
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
    }

    // Update is called once per frame
    void Update () {
        float verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(horizontalMovement, 0, verticalMovement);

        if (Input.GetKeyDown(KeyCode.Space) && teleportsCount > 0)
            teleport();

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

        if (other.CompareTag("Door"))
        {
            int playerPos = rand.Next(0, 10);
            this.transform.position = alcoveCoords[playerPos];
        }

    }


    void teleport()
    {
        if (teleportsCount > 0)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject agent = GameObject.FindGameObjectWithTag("Agent");
            int enemyIndex;
            float enemyDistance = Vector3.Distance(enemies[0].transform.position, transform.position);
            float enemyDistance2 = Vector3.Distance(enemies[1].transform.position, transform.position);
            if (enemyDistance < enemyDistance2)
                enemyIndex = 0;
            else {
                enemyIndex = 1;
                enemyDistance = enemyDistance2;
            }
            float agentDistance = 1000;
            if (agent !=null)
                agentDistance =Vector3.Distance(agent.transform.position, transform.position);

            if (agentDistance < enemyDistance)
                teleportAgent(agent);
            else
                enemies[enemyIndex].GetComponent<EnemyController>().Respawn();

            teleportsCount--;
        }

    }

    private void teleportAgent(GameObject agent)
    {
        int agentPos = rand.Next(0, 10);
        agent.transform.position = alcoveCoords[agentPos];
    }


}
