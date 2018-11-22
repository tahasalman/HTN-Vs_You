using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
public class BodyController : MonoBehaviour {


    public GameObject parent;
    private Random rand = new Random();
    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            parent.GetComponent<EnemyController>().Respawn();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            int num = rand.Next(0, 3);


            if (num == 0)
                parent.GetComponent<EnemyController>().Respawn();
            else if (num == 1)
            {
                parent.GetComponent<EnemyController>().direction *= -1;
                parent.GetComponent<EnemyController>().updateVisionPlane();
            }
            else
                parent.GetComponent<EnemyController>().disableVisionPlane();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
            parent.GetComponent<EnemyController>().enableVisionPlane();
    }

}
