using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCurve : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
	}

	private Animation _animation;

	public GameObject Target ;    //目标对象
	public GameObject[] Items = null;   //目标对象子对象数组
	public float[] PosY = null;         //目标对象子对象的Y坐标

	public float AnimDuration = 0.3f;   //动画所用时间
	public float AnimBackTime = 0.1f;   //动画返回所用时间
	public float AnimInterval = 0.1f;   //相邻物体间动画间隔
	public float InitXPos = -25;        //X坐标初始化位置
	public float MaxXPos = 90;          //X坐标最大位置
	public float DestXPos = 60;         //X坐标目标位置

	// Use this for initialization
	void Start()
	{
		if (Target == null || Items == null || Items.Length <= 0 || Items.Length != PosY.Length)
		{
			return;
		}

		_animation = Target.AddComponent<Animation>();

		var clip = new AnimationClip()
		{
			name = "test",
			legacy = true,
			wrapMode = WrapMode.Once
		};

		for (var i = 0; i < Items.Length; i++)
		{
			var item = Items[i];
			if (item == null)
			{
				continue;
			}

			var relativePath = item.name;

			//创建X轴动画
			var curve = new AnimationCurve();
			curve.AddKey(new Keyframe(0, InitXPos));                                                //初始状态
			curve.AddKey(new Keyframe(0 + AnimInterval * i, InitXPos));                             //暂停关键帧
			curve.AddKey(new Keyframe(0 + AnimInterval * i + AnimDuration, MaxXPos));               //运动关键帧
			curve.AddKey(new Keyframe(AnimInterval * i + AnimDuration + AnimBackTime, DestXPos));   //返回关键帧
			clip.SetCurve(relativePath, typeof(RectTransform), "m_AnchoredPosition.x", curve);

			var curve1 = new AnimationCurve();
			curve1.AddKey(new Keyframe(0, InitXPos));                                                //初始状态
			curve1.AddKey(new Keyframe(0 + AnimInterval * i, InitXPos+20));                             //暂停关键帧
			curve1.AddKey(new Keyframe(0 + AnimInterval * i + AnimDuration*5, MaxXPos));               //运动关键帧
			curve1.AddKey(new Keyframe(AnimInterval * i + AnimDuration*5 + AnimBackTime, DestXPos));   //返回关键帧
			clip.SetCurve(relativePath, typeof(RectTransform), "m_AnchoredPosition.x", curve1);

			//创建Y轴动画
			curve = new AnimationCurve();
			curve.AddKey(new Keyframe(0, PosY[i]));
			curve.AddKey(new Keyframe(AnimDuration + AnimInterval * i, PosY[i]));
			clip.SetCurve(relativePath, typeof(RectTransform), "m_AnchoredPosition.y", curve);

			//创建Scale动画
			curve = new AnimationCurve();
			curve.AddKey(new Keyframe(0, 0));                               //初始状态
			curve.AddKey(new Keyframe(0 + AnimInterval * i, 0));            //暂停状态
			curve.AddKey(new Keyframe(AnimInterval * i + AnimDuration, 1)); //运动状态
			clip.SetCurve(relativePath, typeof(RectTransform), "m_Scale.x", curve);
			clip.SetCurve(relativePath, typeof(RectTransform), "m_Scale.y", curve);
		}

		_animation.AddClip(clip, clip.name);
		_animation.Play(clip.name);
	}
}
