using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testQUeued : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.gameObject.GetComponent<Animation>().PlayQueued("a");
        this.gameObject.GetComponent<Animation>().PlayQueued("b");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
