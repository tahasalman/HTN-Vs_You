using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void onCollisi(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Agent") || other.CompareTag("Player"))
            other.gameObject.SetActive(false);
    }
}
