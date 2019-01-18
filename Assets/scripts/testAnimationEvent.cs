using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnimationEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void toggleVisible() {
        if(this.gameObject.transform.localScale.x == 1) {
            this.gameObject.transform.localScale = Vector3.zero;
        } else {
            this.gameObject.transform.localScale = Vector3.one;
        }
       
    }
}
