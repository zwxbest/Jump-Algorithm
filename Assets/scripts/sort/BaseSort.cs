﻿using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseSort : MonoBehaviour {

    private Color[] colors = new Color[]{new Color(1,0,0),new Color(1,0.6470588235294118f,0)
        ,new Color(1,1,0),new Color(0,1,0),
        new Color(0,0.5f,1),new Color(0,0,1),new Color(0.5450980392156863f,0,1)};

    private System.Random random = new System.Random();

    private Dropdown dataDropDown;

    private Dropdown runDropDown;

    private RunMode curMode = RunMode.ALL;

    protected List<GameObject> elements = new List<GameObject>();

    private float animStepTime = 0.5f;

    private float unitDistance = 60;

    private GameObject elementParent;

    private Dictionary<int, Sequence> roundSeqDic = new Dictionary<int, Sequence>();

    private GameObject elementPrefab;

    protected GameObject pointerPrefab;

    public Sequence globalSequence;

    private float startPositionX = 221;

    private int segHeight = 30;

    private GameObject indexTextPrefab;

    protected GameObject indexTextParent;

    private Button generateBtn;

    private Button runBtn;

    private Slider slider;

    private Text runText;

    private GameObject lastStepOne;

    private GameObject lastRoundOne;

    protected Text inputFieldText;

    private Text error;

    private float tempPosX = 101;

    protected GameObject temp;

    private float initTimeScale = 1f;
   public virtual void Start() {
        indexTextPrefab = (GameObject)Resources.Load("prefabs/index");
        pointerPrefab = (GameObject)Resources.Load("prefabs/PointerPrefab");
        elementPrefab = (GameObject)Resources.Load("prefabs/ElementPrefab");
        runDropDown = GameObject.Find("RunModeDropDown").GetComponent<Dropdown>();
        runText = GameObject.Find("Run/Text").GetComponent<Text>();
        indexTextParent = GameObject.Find("indexTextParent");
        elementParent = GameObject.Find("elementParent");
        generateBtn = GameObject.Find("Generate").GetComponent<Button>();
        runBtn = GameObject.Find("Run").GetComponent<Button>();
        dataDropDown = GameObject.Find("DataDropDown").GetComponent<Dropdown>();
        slider = GameObject.Find("SpeedSlider/Slider").GetComponent<Slider>();
        inputFieldText = GameObject.Find("InputField/Text").GetComponent<Text>();
        slider.onValueChanged.AddListener(OnSliderValueChange);
        generateBtn.onClick.AddListener(Click);
        runBtn.onClick.AddListener(Run);
        dataDropDown.onValueChanged.AddListener(dataDropDownChange);
        error = GameObject.Find("Error").GetComponent<Text>();
        inputFieldText.transform.parent.GetComponent<InputField>().text = "7,6,2,3,4,5,1";
    }

    public void OnSliderValueChange(float value) {
        initTimeScale = 1 + value * 20;
        DOTween.timeScale = initTimeScale;
    }

    public virtual void dataDropDownChange(int i) {
      
        if (i == 4) {
            inputFieldText.transform.parent.localScale = Vector3.one;

        } else {
            inputFieldText.transform.parent.localScale = Vector3.zero;
        }
    }

    public void Run() {
        if (curMode == RunMode.STEP) {
            if (globalSequence.IsActive()) {
                runText.text = "下一步";
                globalSequence.Play();
            } else {
                runText.text = "运行";
            }
        } else if (curMode == RunMode.ROUND) {
            if (globalSequence.IsActive()) {
                runText.text = "下一回合";
                globalSequence.Play();
            } else {
                runText.text = "运行";
            }
        } else {
          globalSequence.Play();
        }
       
   }

    /// <summary>
    /// 从a移动到b，元素，单向的
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b">-1表示移动到temp</param>
    /// <param name="round"></param>
    protected void createOneWayElementAnim(int a, int b, int round) {
        Sequence seq = getOrNewSeq(round);
        if(a == -1) {
            createTempAmin(b, false, round);
            elements[b] = temp;
        }
        else if(b == -1) {
            createTempAmin(a, true, round);
            temp = elements[a];
        } else {
            createElementAnim(a, b, round);
            elements[b] = elements[a];

        }
    }

    protected void createElementAnims(int a, int b, int round) {

        Sequence seq = getOrNewSeq(round);
        for (int i = 0; i < elements.Count; i++) {
            if (i == a) {
                createElementAnim(a, b, round);
                createElementAnim(b, a, round);
                //交换位置
                var temp = elements[a];
                elements[a] = elements[b];
                elements[b] = temp;
            } else if (i == b) {

            } else {
                GameObject element = elements[i];
                int realIndex = int.Parse(element.name.Replace("element", ""));

                float targetValue = element.transform.position.x + (i - realIndex) * unitDistance;
                Tweener t1 = element.transform.DOMoveX(targetValue, animStepTime);
                seq.Append(t1);
              
            }
        }
   }
    private void createElementAnim(int i, int j, int round) {
        GameObject element = elements[i].gameObject;
        Sequence seq = getOrNewSeq(round);
        float targetX =  startPositionX +j * unitDistance;
        Tweener t1 = element.transform.DOMoveX(targetX, animStepTime);
        seq.Append(t1);
    }

    private void createTempAmin(int i,bool toTemp, int round) {
      
        Tweener t;
       Sequence seq = getOrNewSeq(round);
        if (toTemp) {
            GameObject go = elements[i];
            t = go.transform.DOMoveX(tempPosX, animStepTime);
        } else {
            float targetX = startPositionX + i * unitDistance;
            t = temp.transform.DOMoveX(targetX, animStepTime);
   
        }
        seq.Append(t);
    }

    private Sequence getOrNewSeq(int round) {
        Sequence seq;
        if (roundSeqDic.ContainsKey(round)) {
            seq = roundSeqDic[round];
        } else {
            seq = DOTween.Sequence();
            seq.Pause();
            roundSeqDic.Add(round, seq);

        }
        return seq;
    }

    protected Tweener createStepPointerGoAnim(int round, int startIndex, int left, int right, int timeGlobalIndex, int valueGlobalIndex, PointerInfo info) {

        Dictionary<int, Dictionary<int, GameObject>> goDicDic = info.goDicDic;
        GameObject go = goDicDic[round][startIndex];
        Sequence seq = getOrNewSeq(round);
        Vector3 pos = go.transform.position;
        float value = getKeyValue(valueGlobalIndex);
        Tweener tweener = go.transform.DOMoveX(value, animStepTime);

        tweener.OnComplete(() => {
            if (curMode == RunMode.STEP) {
                globalSequence.Pause();
            }
        });
        seq.Append(tweener);

        //第一个时显示,最后一个时隐藏
        if (timeGlobalIndex == left) {
            tweener.OnStart(() => {
                if (info.previousOneDic.ContainsKey(go)) {
                    info.previousOneDic[go].transform.localScale = Vector3.zero;
                }
                go.transform.localScale = Vector3.one;
            });
        }
        return tweener;
    }

    //一个round动一下的动画
    protected Tweener createRoundPointerGoAnim(int round, PointerInfo info) {

        Dictionary<int, GameObject> goDic = info.goDic;
        GameObject go = goDic[round];
        Sequence seq = getOrNewSeq(round);
        Tweener tweener = go.transform.DOMoveX(go.transform.position.x, animStepTime);

        //开始的时候显示
        tweener.OnStart(() => {
            if (info.previousOneDic.ContainsKey(go)) {
                info.previousOneDic[go].transform.localScale = Vector3.zero;
            }
            go.transform.localScale = Vector3.one;
        });
        //结束的时候隐藏
        tweener.OnComplete(() => {
            //go.transform.localScale = Vector3.zero;
        });
        seq.Append(tweener);
        return tweener;
    }

    protected virtual void restart() {
        lastRoundOne = null;
        lastStepOne = null;
        DOTween.KillAll();
        roundSeqDic.Clear();
        generateBtn.GetComponentInChildren<Text>().text = "生成数据";
        for (int i= 0; i < elementParent.transform.childCount;i++) {
            Destroy(elementParent.transform.GetChild(i).gameObject);
        }
        elements.Clear();

    }


    protected virtual void sort(int[] arr) {
    }

    public void Click() {
        if(elements.Count != 0) {
            restart();
        }
        globalSequence = DOTween.Sequence();
        globalSequence.Pause();
        DOTween.SetTweensCapacity(500, 50);
        DOTween.timeScale = initTimeScale;

        DOTween.logBehaviour = LogBehaviour.Verbose;
        if (runDropDown.value == 0) {
            curMode = RunMode.STEP;
        } else if (runDropDown.value == 1) {
            curMode = RunMode.ROUND;
        } else {
            curMode = RunMode.ALL;
        }
        int[] arr = instantiateElement();
        if(arr == null) {
            return;
        }
        sort(arr);
        foreach (var entry in roundSeqDic) {
            if (curMode == RunMode.ROUND) {
                entry.Value.OnComplete(
                    () => {
                        globalSequence.Pause();
                    });
            }
            globalSequence.Append(entry.Value);
        }
    }

    private int[] instantiateElement() {
        int[] arr = new int[7];
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < 7; i++) {
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
               string[] inputs =  inputFieldText.text.Split(new char[] { ',' });
               if(inputs.Length != 7) {
                    error.text = "请输入7个数字!";
                    Tween twneer = error.gameObject.transform.DOScale(1, 3).SetLoops(2,LoopType.Yoyo);
                    return null;
                }
                try {
                    height = int.Parse(inputs[i]);
                    if(height > 7) {
                        throw new Exception("最大不能大于7");
                    }
                } catch (Exception e) {
                    error.text = e.Message;
                    Tween twneer = error.gameObject.transform.DOScale(1, 3).SetLoops(2, LoopType.Yoyo);
                    return null;
                }
          
            }
            GameObject instance = Instantiate(elementPrefab);
            instance.transform.SetParent(elementParent.transform);
            Transform elementIamge = instance.transform.Find("elementImage");
            elementIamge.name = "element" + i.ToString();
            instance.transform.localPosition = new Vector3(-179 + 60 * i, 50, 0);
            elementIamge.GetComponent<Image>().color = colors[i % 7];
            arr[i] = height;
            instance.transform.Find("indexText").GetComponent<Text>().text = i.ToString();
            elementIamge.GetComponent<RectTransform>().sizeDelta = new Vector2(33, height * segHeight);
            elementIamge.Find("num").GetComponent<Text>().text = height.ToString();
            //元素集合
            elements.Add(elementIamge.gameObject);
        }
        return arr;
    }
    protected GameObject newRoundPointerGo(int round, int globalIndex, PointerInfo info) {
        GameObject go = Instantiate(pointerPrefab);
        go.name = info.baseName + "_" + round + "_" + globalIndex;
        go.GetComponent<Text>().text = info.baseName;
        go.transform.SetParent(info.goParent.transform);
        Vector3 pos = elements[globalIndex].transform.position;
        go.transform.position = new Vector3(startPositionX + globalIndex * 60, pos.y + info.yOffset, pos.z);
        Vector3 rotation = go.transform.rotation.eulerAngles;
        go.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, info.rotation);
        go.transform.localScale = Vector3.zero;
        info.goDic.Add(round, go);
        if (lastRoundOne != null) {
            info.previousOneDic.Add(go, lastRoundOne);
        }
        lastRoundOne = go;
        return go;
    }

    protected GameObject newStepPointerGo(int round, int globalIndex, PointerInfo info) {
        GameObject go = Instantiate(pointerPrefab);
        go.name = info.baseName + "_" + round + "_" + globalIndex;
        go.GetComponent<Text>().text = info.baseName;
        go.transform.SetParent(info.goParent.transform);
        Vector3 pos = elements[0].transform.position;
        go.transform.position = new Vector3(startPositionX + globalIndex * 60, pos.y + info.yOffset, pos.z);
        go.transform.localScale = Vector3.zero;
        go.transform.FindChild("Image").transform.localEulerAngles = new Vector3(0,0, info.rotation);
        Dictionary<int, GameObject> goDic;
        if (info.goDicDic.ContainsKey(round)) {
            goDic = info.goDicDic[round];
        } else {
            goDic = new Dictionary<int, GameObject>();
            info.goDicDic.Add(round, goDic);
        }
        goDic.Add(globalIndex, go);
        if (lastStepOne != null) {
            info.previousOneDic.Add(go, lastStepOne);
        }
        lastStepOne = go;
        return go;
    }

    public void swap(int[] arr, int i, int j,int round) {
        int t = arr[i];
        arr[i] = arr[j];
        arr[j] = t;
   }

    private float getKeyValue(int globalIndex) {
        float diff = globalIndex * unitDistance;
        return startPositionX + diff;
    }


}