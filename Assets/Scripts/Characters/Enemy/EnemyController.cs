using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class EnemyController : MonoBehaviour {

    public float speed = 10.0F;
    public GameObject visionPlane;

    public int direction;
    private float charZ;
    private float charY;
    private float visionX;
    private float visionY;
    private float visionZ;
    //private bool isColliding = false;
    private Random rand;

    // Use this for initialization
    void Start()
    {
        rand = new Random();

        charZ = transform.position.z;
        charY = transform.position.y;
        visionX = visionPlane.transform.localPosition.x;
        visionY = visionPlane.transform.localPosition.y;
        visionZ = visionPlane.transform.localPosition.z;

        if (transform.position.x < 0)
        {
            direction = 1;
            updateVisionPlane();
        }
        else
        {
            direction = -1;
            updateVisionPlane();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //isColliding = false;
        float horizontalMovement = direction * speed * Time.deltaTime;
        transform.Translate(horizontalMovement, 0, 0);

    }

    public void Respawn()
    {
        int num = rand.Next(0, 2);
        if (num == 0)
        {
            transform.position = new Vector3(-78.5F, charY,charZ);
            direction = 1;
            updateVisionPlane();
        }
        else
        {
            transform.position = new Vector3(78.5F, charY, charZ);
            direction = -1;
            updateVisionPlane();
        }
    }

    public void updateVisionPlane() {
        visionPlane.transform.localPosition = new Vector3(visionX * direction, visionY, visionZ);
    }
    
    public void disableVisionPlane()
    {
        visionPlane.SetActive(false);
    }

    public void enableVisionPlane()
    {
        visionPlane.SetActive(true);
    }

}
