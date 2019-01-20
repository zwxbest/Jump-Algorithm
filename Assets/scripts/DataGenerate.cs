using Assets.scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum PointerEnum {
   INDEX_TEXT,
   PARTITION_VALUE,
   PARTITION_POS
};

public class DataGenerate : MonoBehaviour {

    public GameObject test;

    public GameObject partitionValuePrefab;

    public GameObject partitionValueParent;

    public GameObject partitionPosPrefab;

    public GameObject partitionPosParent;

    public GameObject indexTextParent;

	public GameObject elementPrefab;

	public Dropdown dataDropDown;

    public GameObject[] fixedElements;

    private float startPositionX = -180;

	public GameObject elementParent;

	private System.Random random  = new System.Random();

	private int segHeight = 30;

    public GameObject indexTextPrefab;

	private Color[] colors = new Color[]{new Color(1,0,0),new Color(1,0.6470588235294118f,0)
		,new Color(1,1,0),new Color(0,1,0),
		new Color(0,0.5f,1),new Color(0,0,1),new Color(0.5450980392156863f,0,1)};


	private int total = 7;

    public GameObject parititonGo;

	private float animStepTime = 1f;

    private string positionX = "m_AnchoredPosition.x";
    
    private List<GameObject> elements = new List<GameObject>();

    //value的dic
    private Dictionary<int, GameObject> partitionValueDic = new Dictionary<int, GameObject>();
    //partition的dic
    private Dictionary<int, GameObject> partitionDic = new Dictionary<int, GameObject>();

    private Dictionary<int,int> roundLengthDic = new Dictionary<int, int>();

    private Dictionary<int,int> roundLengthSumDic = new Dictionary<int, int>();

    private string clipName = "clip";

    private float unitDistance = 60;

    //key为clip，value的key为curve的属性名，value为curve
    private Dictionary<AnimationClip, Dictionary<string, AnimationCurve>> clipCurveDic = new Dictionary<AnimationClip, Dictionary<string, AnimationCurve>>();

    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> indexTextDicDic= new Dictionary<int, Dictionary<int, GameObject>>();
    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> partitionPosDicDic= new Dictionary<int, Dictionary<int, GameObject>>();
    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> partitionValueDicDic= new Dictionary<int, Dictionary<int, GameObject>>();

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
      
    }


    private int[] instantiateElement() {
        int[] arr = new int[total];
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        if (gos != null) {
            foreach (GameObject go in gos) {
                GameObject.Destroy(go);
            }
        }
        for (int i = 0; i < 7; i++) {
            GameObject instance = Instantiate(elementPrefab);
            instance.transform.SetParent(elementParent.transform);
            Transform elementIamge = instance.transform.Find("elementImage");
            elementIamge.name = "element" + i.ToString();
            instance.transform.localPosition = new Vector3(-179 + 60 * i, 50, 0);
            elementIamge.GetComponent<Image>().color = colors[i % 7];
            int height = 0;
            if (dataDropDown.value == 0) {
                height = random.Next(1, 8);
            } else if (dataDropDown.value == 1) {
                height = 4;
            } else if (dataDropDown.value == 2) {
                height = i + 1;
            } else if (dataDropDown.value == 3) {
                height = 7 - i;
            } else if (dataDropDown.value == 4) {
                arr = new int[] { 7, 6, 2, 3, 4, 5, 1 };
                height = arr[i];
            }
            arr[i] = height;
            instance.transform.Find("indexText").GetComponent<Text>().text = i.ToString();
            elementIamge.GetComponent<RectTransform>().sizeDelta = new Vector2(33, height * segHeight);
            elementIamge.Find("num").GetComponent<Text>().text = height.ToString();
            //元素集合
            elements.Add(elementIamge.gameObject);
        }
        fixedElements = new GameObject[elements.Count];
        Array.Copy(elements.ToArray(), fixedElements, elements.Count);
        return arr;
    }


	public void Click(){
     
        int[] arr = instantiateElement();

        int[] copy = new int[arr.Length];
        Array.Copy(arr, copy, arr.Length);
        QuickSortForRound quickSortForRound = new QuickSortForRound();
        quickSortForRound.sort(arr);
        roundLengthDic = quickSortForRound.getRoundLengthDic();
        roundLengthSumDic = quickSortForRound.getRoundLengthSum();
        sort(copy);
        playAnim();
    }

    private void playAnim() {

        GameObject[] gos1 = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gos1) {
            var animation = go.GetComponent<Animation>();
            if (animation != null && animation.GetClipCount() != 0) {
                go.GetComponent<Animation>().PlayQueued(clipName);
            }
        }
        foreach (var indexTextDic in indexTextDicDic) {
            foreach (var indexText in indexTextDic.Value) {
                indexText.Value.gameObject.GetComponent<Animation>().PlayQueued("clip");
            }
        }
        foreach (var dic in partitionValueDicDic) {
            foreach (var go in dic.Value) {
                go.Value.gameObject.GetComponent<Animation>().PlayQueued("clip");
            }
        }

        foreach (var dic in partitionPosDicDic) {
            foreach (var go in dic.Value) {
                go.Value.gameObject.GetComponent<Animation>().PlayQueued("clip");
            }
        }
    }


    private void sort(int[] arr){
		quickSort (arr, 0, arr.Length - 1, 1);
	}

	public void quickSort(int[] arr, int l, int r, int round) {
		if (l >= r) {
			return;
		}
		int p = partition(arr, l, r, round);
		quickSort(arr, l, p - 1, round + 1);
		quickSort(arr, p + 1, r, round + 1);
	}

    private void fixElementPos(int round,int indexInRound) {
        for(int i= 0;i <elements.Count;i++) {
            GameObject element = elements[i];
            int realIndex = int.Parse(element.name.Replace("element", ""));
            var animation  = getOrNewAnimation(element);
            var clip = getOrNewClip(animation, clipName);
            var curve = getOrNewCurves(clip, new Type[] { typeof(RectTransform) }, new string[] { positionX }, new float[] { element.GetComponent<RectTransform>().anchoredPosition.x })[positionX];
            float startTime = round == 1 ? 0 : roundLengthSumDic[round - 1] * animStepTime;
            float keyTime = startTime + animStepTime * indexInRound;
            curve.AddKey(keyTime, element.GetComponent<RectTransform>().anchoredPosition.x+(i-realIndex)*unitDistance);
            //Debug.Log(element.name + " "+(i * unitDistance));
            clip.SetCurve("", typeof(RectTransform), positionX, curve);
        }
    }

    int partition(int[] arr, int left, int right, int round) {
		int v = arr[left];
		int partition = left;
        int i = left + 1;
        int indexInRound = 0;
        var indexText = getPointerGo(round, i,PointerEnum.INDEX_TEXT);
        var partitionValue = getPointerGo(round, left,PointerEnum.PARTITION_VALUE);
        var partitionPos = getPointerGo(round, left+1, PointerEnum.PARTITION_POS);
        for (; i <= right; i++,indexInRound++) {
            createPointerGoAnim(round,left+1,right,i,i,PointerEnum.INDEX_TEXT);
            createPointerGoAnim(round, left, right,i-1,left, PointerEnum.PARTITION_VALUE);
            createPointerGoAnim(round, left+1, right, i, partition, PointerEnum.PARTITION_POS);
            fixElementPos(round,indexInRound);
            if (arr[i] < v) {
                indexInRound++;
                partition++;
                swap(arr, partition, i);
                createSwapAnim(i,partition, round, left + 1, indexInRound);
            }
            
        }

		if (left != partition) {
            indexInRound++;
			swap(arr, left, partition);
            createSwapAnim(left, partition, round, left + 1, indexInRound);
        }
		return partition;
	}

	private void swap(int[] arr, int i, int j) {
		int t = arr[i];
		arr[i] = arr[j];
		arr[j] = t;

    }
    private float getKeyTime(int round, int globalIndex, int indexInRound) {
        float startTime = round == 1 ? 0 : roundLengthSumDic[round - 1] * animStepTime;
        float keyTime = startTime + animStepTime * indexInRound;
        return keyTime;
    }

    private float getKeyValue(int globalIndex) {
        float diff = globalIndex * unitDistance;
        return startPositionX + diff;
    }

    private GameObject getPointerGo(int round, int globalIndex,PointerEnum pointer) {
        GameObject prefab = null;
        GameObject goParent = null;
        int yOffset = 0;
        string goName = null;
        Dictionary<int,Dictionary<int,GameObject>> goDicDic = null;
        switch (pointer) {
            case PointerEnum.INDEX_TEXT:
                prefab = indexTextPrefab;
                goParent = indexTextParent;
                yOffset = -120;
                goDicDic = indexTextDicDic;
                goName = "indexText";
                break;
            case PointerEnum.PARTITION_VALUE:
                prefab = partitionValuePrefab;
                goParent = partitionValueParent;
                yOffset = 200;
                goDicDic = partitionValueDicDic;
                goName = "parititionValue";
                break;
            case PointerEnum.PARTITION_POS:
                prefab = partitionPosPrefab;
                goParent = partitionPosParent;
                yOffset = 200;
                goDicDic = partitionPosDicDic;
                goName = "partitionPos";
                break;
        }
        GameObject go = Instantiate(prefab);
        go.name = goName + "_" + round + "_" + globalIndex;
        go.transform.SetParent(goParent.transform);
        GameObject element = elements[globalIndex];
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPositionX+globalIndex*60, element.GetComponent<RectTransform>().anchoredPosition.y + yOffset);
        go.transform.localScale = Vector3.zero;
        Dictionary<int, GameObject> goDic;
        if (goDicDic.ContainsKey(round)) {
            goDic = goDicDic[round];
        } else {
            goDic = new Dictionary<int, GameObject>();
            goDicDic.Add(round, goDic);
        }
        goDic.Add(globalIndex, go);
        return go;
    }


    private void createPointerGoAnim(int round, int startIndex, int right, int timeGlobalIndex,int valueGlobalIndex,PointerEnum pointer) {
        Dictionary<int, Dictionary<int, GameObject>> goDicDic = null;
        switch (pointer) {
            case PointerEnum.INDEX_TEXT:
                goDicDic = indexTextDicDic;
                break;
            case PointerEnum.PARTITION_VALUE:
                goDicDic = partitionValueDicDic;
                break;
            case PointerEnum.PARTITION_POS:
                goDicDic = partitionPosDicDic;
                break;
        }

        GameObject go = goDicDic[round][startIndex];
        Animation animation = getOrNewAnimation(go);
        AnimationClip clip = getOrNewClip(animation, clipName);
        AnimationCurve curve = getOrNewCurves(clip, new Type[] { typeof(RectTransform) }, new string[] { positionX }, new float[] { 0f })[positionX];
        float time = getKeyTime(round, timeGlobalIndex, timeGlobalIndex - startIndex);
        float value = getKeyValue(valueGlobalIndex);
        if(go.name == "partitionPos_1_1") {
            Debug.Log(string.Format("{0} _ {1}", time, value));
        }
        if (time == 0f) {
            curve.RemoveKey(0);
        }
        curve.AddKey(time, value);
        clip.SetCurve("", typeof(RectTransform), positionX, curve);
        //第一个时显示,最后一个时隐藏
        if (timeGlobalIndex == startIndex || timeGlobalIndex == right) {
            AnimationEvent showEvent = new AnimationEvent();
            showEvent.functionName = "toggleVisible";
            showEvent.time = time;
            clip.AddEvent(showEvent);
        }
    }



	private void createSwapAnim(int i,int j,int round,int startIndexInRound,int indexInRound){
		createElementAnim(i,j,round, startIndexInRound,indexInRound);
		createElementAnim (j, i,round, startIndexInRound, indexInRound);
        //交换位置
        var temp = elements[i];
        elements[i] = elements[j];
        elements[j] = temp;
	}

	private void createElementAnim(int i,int j,int round,int startIndexInRound,int indexInRound){
        GameObject element = elements[i].gameObject;
        //实际的index
        int realIndex = int.Parse(element.name.Replace("element", ""));

        Animation animation = getOrNewAnimation(element);
		AnimationClip clip = getOrNewClip(animation,clipName);
        AnimationCurve curve  = getOrNewCurves(clip, new Type[] { typeof(RectTransform) }, new string[] { positionX },new float[] { 0} )[positionX];
        float startTime = round == 1 ? 0 : roundLengthSumDic[round - 1] * animStepTime;
        float keyTime = startTime + animStepTime * indexInRound;
        int diff = (j - realIndex) * 60;
        curve.AddKey(keyTime,element.GetComponent<RectTransform>().anchoredPosition.x+diff);
		clip.SetCurve("", typeof(RectTransform), positionX, curve);
	}


    private Animation getOrNewAnimation(GameObject go) {
        Animation animation = go.GetComponent<Animation>();
        if (animation == null) {
            animation = go.AddComponent<Animation>();
            animation.wrapMode = WrapMode.Once;
        }
        return animation;
    }

    private AnimationClip getOrNewClip(Animation animation,String clipName) {
        AnimationClip clip = animation.GetClip(clipName);
        if ( clip == null) {
            clip = new AnimationClip();
            clip.name = clipName;
            clip.wrapMode = WrapMode.Once;
            clip.legacy = true;
            animation.AddClip(clip, clipName);
        }
        return clip;
    }

    private Dictionary<string,AnimationCurve> getOrNewCurves(AnimationClip clip,Type[] types, string[] curveTargets,float[] initValues) {
        if(curveTargets.Length != initValues.Length) {
            throw new Exception("target和value数量必须相等");
        }
        if(curveTargets.Length != types.Length) {
            throw new Exception("target和types数量必须相等");
        }
        Dictionary<string, AnimationCurve> curveDic;
        if (clipCurveDic.ContainsKey(clip)) {
            curveDic = clipCurveDic[clip];
            for(int i=0;i<curveTargets.Length;i++) {
                if (!curveDic.ContainsKey(curveTargets[i])) {
                    var curve = new AnimationCurve();
                    curve.AddKey(0, initValues[i]);
                    clip.SetCurve("", types[i], curveTargets[i], curve);
                    curveDic = clipCurveDic[clip];
                    curveDic.Add(curveTargets[i], curve);
                }
            }
        } else {
            curveDic = new Dictionary<string, AnimationCurve>();
            for (int i = 0; i < curveTargets.Length; i++) {
                var curve = new AnimationCurve();
                curve.AddKey(0, initValues[i]);
                clip.SetCurve("", types[i], curveTargets[i], curve);
                curveDic.Add(curveTargets[i], curve);
            }
            clipCurveDic.Add(clip, curveDic);
        }
        return curveDic;
    }
}
