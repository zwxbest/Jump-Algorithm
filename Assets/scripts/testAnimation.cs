using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnimation : MonoBehaviour {

    public GameObject elemnet1;

	// Use this for initialization
	void Start () {
        Animation animation = this.GetComponent<Animation>();
        AnimationClip clip = new AnimationClip();
        clip.name = "a";
        clip.wrapMode = WrapMode.Once;
        clip.legacy = true;
        animation.AddClip(clip,"a");

        //clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve1);

        AnimationCurve curve1 = new AnimationCurve();
        curve1.AddKey(new Keyframe(1f, -100));
        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve1);
        curve1.AddKey(new Keyframe(2f, -50));
        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve1);
        animation.PlayQueued("a");

        Animation animation1 = elemnet1.GetComponent<Animation>();
        AnimationClip clip1 = new AnimationClip();
        clip1.name = "a";
        clip1.wrapMode = WrapMode.Once;
        clip1.legacy = true;
        animation1.AddClip(clip1, "a");

        //clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve1);

        AnimationCurve curve2 = new AnimationCurve();
        curve2.AddKey(new Keyframe(1f, -100));
        clip1.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve2);
        curve2.AddKey(new Keyframe(2f, -50));
        clip1.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", curve2);
        animation1.PlayQueued("a");



    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
