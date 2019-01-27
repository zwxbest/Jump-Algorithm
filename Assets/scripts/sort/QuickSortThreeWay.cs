using DG.Tweening;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class QuickSortThreeWay : MonoBehaviour {
    enum PointerEnum {
        INDEX,
        LT,
        GT,
    };

    private GameObject pointerPrefab;

    public GameObject IndexParent;

    public GameObject LtParent;

    public GameObject GtParent;

    private GameObject elementPrefab;

    private RunMode curMode = RunMode.ALL;

    public Dropdown dataDropDown;

    private float startPositionX = 221;

    public GameObject elementParent;

    private System.Random random = new System.Random();

    private int segHeight = 30;

    public Sequence globalSequence;

    private Color[] colors = new Color[]{new Color(1,0,0),new Color(1,0.6470588235294118f,0)
        ,new Color(1,1,0),new Color(0,1,0),
        new Color(0,0.5f,1),new Color(0,0,1),new Color(0.5450980392156863f,0,1)};

    private float animStepTime = 0.5f;

    private List<GameObject> elements = new List<GameObject>();

    private Dictionary<int, Sequence> roundSeqDic = new Dictionary<int, Sequence>();

    private float unitDistance = 60;

    public Text runText;

    public Dropdown runDropDown;

    //key为clip，value的key为curve的属性名，value为curve
    private Dictionary<AnimationClip, Dictionary<string, AnimationCurve>> clipCurveDic = new Dictionary<AnimationClip, Dictionary<string, AnimationCurve>>();

    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> indexDicDic = new Dictionary<int, Dictionary<int, GameObject>>();
    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> LTDicDic = new Dictionary<int, Dictionary<int, GameObject>>();
    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> GTDicDic = new Dictionary<int, Dictionary<int, GameObject>>();

    void Update() {
       
    }

    //i等于-1时表示原地等候，i！=-1表示和j交换，j和i交换
    private void createElementAnims(int a, int b, int round) {
     
        if (a == -1) {
            Sequence seq = getOrNewSeq(round);
            for (int i = 0; i < elements.Count; i++) {
                GameObject element = elements[i];
                int realIndex = int.Parse(element.name.Replace("element", ""));
              
                float targetValue = element.transform.position.x + (i - realIndex) * unitDistance;
                Tweener t1 = element.transform.DOMoveX(targetValue, animStepTime);
                t1.OnStart(() => {
                    if (element.transform.position.x == targetValue) { t1.Complete(); Debug.Log(element.name + "complete"); } });
                seq.Append(t1);
            }
        } else {
            createElementAnim(a, b, round);
            createElementAnim(b, a, round);
            //交换位置
            var temp = elements[a];
            elements[a] = elements[b];
            elements[b] = temp;
        }

    }

    public void OnSliderValueChange(float value) {
        animStepTime = 1 - value + 0.01f;
    }

    private void createElementAnim(int i, int j, int round) {
        GameObject element = elements[i].gameObject;
        //实际的index
        int realIndex = int.Parse(element.name.Replace("element", ""));
        Sequence seq = getOrNewSeq(round);
        Tweener t1 = element.transform.DOMoveX(element.transform.position.x + (j - realIndex) * unitDistance, animStepTime);
        seq.Append(t1);
    }

    void Start() {
        DOTween.defaultAutoPlay = AutoPlay.None;
        pointerPrefab = (GameObject)Resources.Load("prefabs/PointerPrefab");
        elementPrefab = (GameObject)Resources.Load("prefabs/ElementPrefab");
       
       
    }

    private int[] instantiateElement() {
        int[] arr = new int[7];
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        if (gos != null) {
            foreach (GameObject go in gos) {
                Destroy(go);
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
        return arr;
    }

    // Update is called once per frame

    public void Click() {
        globalSequence = DOTween.Sequence();
        if(runDropDown.value == 0) {
            curMode = RunMode.STEP;
        }else if(runDropDown.value == 1) {
            curMode = RunMode.ROUND;
        } else {
            curMode = RunMode.ALL;
        }
        int[] arr = instantiateElement();
        sort(arr);
        foreach (var entry in roundSeqDic) {
            if(curMode == RunMode.ROUND) {
                entry.Value.OnComplete(
                    () => {
                        globalSequence.Pause();
                    });
            }
            globalSequence.Append(entry.Value);
           
        }
    }

    public void Run() {
        if(curMode == RunMode.STEP) {
            if (globalSequence.IsActive()) {
                runText.text = "运行";
            } else {
                runText.text = "下一步";
            }
        }else if(curMode == RunMode.ROUND) {
            if (globalSequence.IsActive()) {
                runText.text = "运行";
            } else {
                runText.text = "下一回合";
            }
        }
        Debug.Log("run");
        globalSequence.Play();
    }


    private void sort(int[] arr) {
        quickSort(arr, 0, arr.Length - 1, 1);
    }

    public void quickSort(int[] arr, int left, int right, int round) {
        if (left >= right) {
            return;
        }
        //int randomIndex = (int)(random.NextDouble() * (right - left + 1)) + left;
        //swap(arr, left, randomIndex,round);

        int v = arr[left];
        int partition = left;

        int lt = left;     // lt+1永远存放第一个<=v的值
        int gt = right + 1; //gt+1存放最后一个>=v的值
        int i = left + 1;    // arr[lt+1...i) == v

        newPointer(round, left+1, PointerEnum.INDEX);
        newPointer(round, lt, PointerEnum.LT);
        newPointer(round, gt, PointerEnum.GT);

        int timeIndex = 0;
        while (i < gt) {
            createPointerGoAnim(round, left + 1, left + 1, right, left + 1 + timeIndex, i, PointerEnum.INDEX);
            //createPointerGoAnim(round, left, left, right, left + 1 + timeIndex, i, PointerEnum.LT);
            //createPointerGoAnim(round, right + 1, left, right, right + 1 - timeIndex, i, PointerEnum.GT);
            if (arr[i] < v) {
                swap(arr, i++, lt++, round);
            } else if (arr[i] > v) {
                //gt交换过来的数不知道大小，下一轮继续比较这个数字
                swap(arr, i, --gt, round);
            } else {
                i++;
            }
            timeIndex++;
        }
        //lt表示第一个等于v的
        swap(arr, 1, lt, round);

        quickSort(arr, left,lt-1, round + 1);
        //因为gt是大于v的第二个元素，gt--是第一个
        quickSort(arr, gt, right, round + 1);
    }

    private void  newPointer(int round, int globalIndex, PointerEnum pointer) {
        GameObject prefab = pointerPrefab;
        GameObject goParent = null;
        int yOffset = 0;
        string goName = null;
        Dictionary<int, Dictionary<int, GameObject>> goDicDic = null;
        switch (pointer) {
            case PointerEnum.INDEX:
                goParent = IndexParent;
                yOffset = 240;
                goDicDic = indexDicDic;
                goName = "index_pointer";
                break;
            case PointerEnum.LT:
                goParent = LtParent;
                yOffset = 180;
                goDicDic = LTDicDic;
                goName = "ltValue";
                break;
            case PointerEnum.GT:
                goParent = GtParent;
                yOffset = 180;
                goDicDic = GTDicDic;
                goName = "gtValue";
                break;
        }
        GameObject go = Instantiate(prefab);
        go.GetComponent<Text>().text = "i";
        go.name = goName + "_" + round + "_" + globalIndex;
        go.transform.SetParent(goParent.transform);
        Vector3 pos = elements[0].transform.position;
        go.transform.position = new Vector3(startPositionX + globalIndex * 60,pos.y + yOffset,pos.z);
        go.transform.localScale = Vector3.zero;
        Dictionary<int, GameObject> goDic;
        if (goDicDic.ContainsKey(round)) {
            goDic = goDicDic[round];
        } else {
            goDic = new Dictionary<int, GameObject>();
            goDicDic.Add(round, goDic);
        }
        goDic.Add(globalIndex, go);
      
    }

    private float getKeyValue(int globalIndex) {
        float diff = globalIndex * unitDistance;
        return startPositionX + diff;
    }


    private void createPointerGoAnim(int round, int startIndex, int left,int right, int timeGlobalIndex, int valueGlobalIndex, PointerEnum pointer) {
        Dictionary<int, Dictionary<int, GameObject>> goDicDic = null;
        switch (pointer) {
            case PointerEnum.INDEX:
                goDicDic = indexDicDic;
                break;
            case PointerEnum.LT:
                goDicDic = LTDicDic;
                break;
            case PointerEnum.GT:
                goDicDic = GTDicDic;
                break;
        }
        GameObject go = goDicDic[round][startIndex];
        Sequence seq = getOrNewSeq(round);
        Vector3 pos = go.transform.position;
        float value = getKeyValue(valueGlobalIndex);
        Tweener tweener = go.transform.DOMoveX(value, animStepTime);

        tweener.OnComplete(() => {
            if(curMode == RunMode.STEP) {
                globalSequence.Pause();
            }
        });
        Debug.Log(string.Format("name is {0},left is {1},right is {2},timeGlobalIndex is {3}", go.name, left, right, timeGlobalIndex));

        seq.Append(tweener);
        //第一个时显示,最后一个时隐藏
        if (timeGlobalIndex == left) {
            tweener.OnStart(() => {
                Debug.Log("one");
                go.transform.localScale = Vector3.one;
            });
        }
        if (timeGlobalIndex == right) {
            tweener.OnComplete(() => {
                Debug.Log("one");
                go.transform.localScale = Vector3.zero;
            });
        }
    }
        

    private Sequence getOrNewSeq(int round) {
        Sequence seq;
        if (roundSeqDic.ContainsKey(round)) {
            seq = roundSeqDic[round];
        } else {
            seq = DOTween.Sequence();
            roundSeqDic.Add(round, seq);
          
        }
        return seq;
    }

    private void swap(int[] arr, int i, int j,int round) {
        int t = arr[i];
        arr[i] = arr[j];
        arr[j] = t;
        //createElementAnims(i, j, round);

    }

}
