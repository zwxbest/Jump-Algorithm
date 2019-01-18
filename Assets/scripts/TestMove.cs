using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Animation animation = this.gameObject.GetComponent<Animation>();
        AnimationClip clip;
        clip = new AnimationClip();
        clip.name = "a";
        clip.wrapMode = WrapMode.Once;
        clip.legacy = true;
        animation.AddClip(clip, clip.name);

        AnimationCurve curve = null;
        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, this.transform.position.x ));
        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve);
        animation.Play("a");
        curve.AddKey(new Keyframe(10, this.transform.position.x + 1000));
        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
