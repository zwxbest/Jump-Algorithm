﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class QuickSortRandom : MonoBehaviour {
    enum PointerEnum {
        INDEX_TEXT,
        PARTITION_VALUE,
        PARTITION_POS
    };

    enum RUN_MODE {
        STEP,
        ROUND,
        ALL
    };


    private bool canRun = false;

    public GameObject partitionValuePrefab;

    public GameObject partitionValueParent;

    public GameObject partitionPosPrefab;

    public GameObject partitionPosParent;

    public GameObject indexTextParent;

    public GameObject elementPrefab;

    private RUN_MODE curMode = RUN_MODE.ALL;

    public Dropdown dataDropDown;

    private float startPositionX = 221;

    public GameObject elementParent;

    private System.Random random = new System.Random();

    private int segHeight = 30;

    public GameObject indexTextPrefab;

    public Sequence globalSequence;

    private Color[] colors = new Color[]{new Color(1,0,0),new Color(1,0.6470588235294118f,0)
        ,new Color(1,1,0),new Color(0,1,0),
        new Color(0,0.5f,1),new Color(0,0,1),new Color(0.5450980392156863f,0,1)};

    private float animStepTime = 0.5f;

    private List<GameObject> elements = new List<GameObject>();

    //value的dic
    private Dictionary<int, GameObject> partitionValueDic = new Dictionary<int, GameObject>();
    //partition的dic
    private Dictionary<int, GameObject> partitionDic = new Dictionary<int, GameObject>();

    private Dictionary<int, Sequence> roundSeqDic = new Dictionary<int, Sequence>();

    private float unitDistance = 60;

    public Text runText;

    public Dropdown runDropDown;

    //key为clip，value的key为curve的属性名，value为curve
    private Dictionary<AnimationClip, Dictionary<string, AnimationCurve>> clipCurveDic = new Dictionary<AnimationClip, Dictionary<string, AnimationCurve>>();

    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> indexTextDicDic = new Dictionary<int, Dictionary<int, GameObject>>();
    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> partitionPosDicDic = new Dictionary<int, Dictionary<int, GameObject>>();
    //第一个key为round，第二个key为全局的index
    private Dictionary<int, Dictionary<int, GameObject>> partitionValueDicDic = new Dictionary<int, Dictionary<int, GameObject>>();

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
        globalSequence = DOTween.Sequence();
        DOTween.defaultAutoPlay = AutoPlay.None;
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
        if(runDropDown.value == 0) {
            curMode = RUN_MODE.STEP;
        }else if(runDropDown.value == 1) {
            curMode = RUN_MODE.ROUND;
        } else {
            curMode = RUN_MODE.ALL;
        }
        int[] arr = instantiateElement();
        sort(arr);
        foreach (var entry in roundSeqDic) {
            if(curMode == RUN_MODE.ROUND) {
                entry.Value.OnComplete(
                    () => {
                        globalSequence.Pause();
                    });
            }
            globalSequence.Append(entry.Value);
        }
    }

    public void Run() {
        if(curMode == RUN_MODE.STEP) {
            if (globalSequence.IsActive()) {
                runText.text = "运行";
            } else {
                runText.text = "下一步";
            }
        }else if(curMode == RUN_MODE.ROUND) {
            if (globalSequence.IsActive()) {
                runText.text = "运行";
            } else {
                runText.text = "下一回合";
            }
        }
        globalSequence.Play();
    }


    private void sort(int[] arr) {
        quickSort(arr, 0, arr.Length - 1, 1);
    }

    public void quickSort(int[] arr, int l, int r, int round) {
        if (l >= r) {
            return;
        }
        int p = partition(arr, l, r, round);
        quickSort(arr, l, p - 1, round + 1);
        quickSort(arr, p + 1, r, round + 1);
    }

    private GameObject getPointerGo(int round, int globalIndex, PointerEnum pointer) {
        GameObject prefab = null;
        GameObject goParent = null;
        int yOffset = 0;
        string goName = null;
        Dictionary<int, Dictionary<int, GameObject>> goDicDic = null;
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
                yOffset = 240;
                goDicDic = partitionValueDicDic;
                goName = "parititionValue";
                break;
            case PointerEnum.PARTITION_POS:
                prefab = partitionPosPrefab;
                goParent = partitionPosParent;
                yOffset = 180;
                goDicDic = partitionPosDicDic;
                goName = "partitionPos";
                break;
        }
        GameObject go = Instantiate(prefab);
        go.name = goName + "_" + round + "_" + globalIndex;
        go.transform.SetParent(goParent.transform);
        Vector3 pos = elements[globalIndex].transform.position;
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
        return go;
    }

    private float getKeyValue(int globalIndex) {
        float diff = globalIndex * unitDistance;
        return startPositionX + diff;
    }


    private void createPointerGoAnim(int round, int startIndex, int left,int right, int timeGlobalIndex, int valueGlobalIndex, PointerEnum pointer) {
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
        Sequence seq = getOrNewSeq(round);
        Vector3 pos = go.transform.position;
        float value = getKeyValue(valueGlobalIndex);
        Tweener tweener = go.transform.DOMoveX(value, animStepTime);

        tweener.OnComplete(() => {
            if(curMode == RUN_MODE.STEP) {
                globalSequence.Pause();
            }
        });
        seq.Append(tweener);
        //第一个时显示,最后一个时隐藏
        if (timeGlobalIndex == left) {
            tweener.OnStart(() => {
                go.transform.localScale = Vector3.one;
            });
        }
        if (timeGlobalIndex == right) {
            tweener.OnComplete(() => {
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

    int partition(int[] arr, int left, int right, int round) {

        int randomIndex = (int)(random.NextDouble() * (right - left + 1)) + left;
        swap(arr, left, randomIndex);
        createElementAnims(left, randomIndex, round);

        int v = arr[left];
        int partition = left;
        
        int indexInRound = 0;
        var indexText = getPointerGo(round, left+1, PointerEnum.INDEX_TEXT);
        var partitionValue = getPointerGo(round, left, PointerEnum.PARTITION_VALUE);
        var partitionPos = getPointerGo(round, partition, PointerEnum.PARTITION_POS);
        // 随机在arr[l...r]的范围中, 选择一个数值作为标定点pivot


        for (int i = left + 1; i <= right; i++, indexInRound++) {
            createPointerGoAnim(round, left,left+1, right, i-1, left, PointerEnum.PARTITION_VALUE);
            createPointerGoAnim(round, left, left+1,right, i, partition, PointerEnum.PARTITION_POS);
            createPointerGoAnim(round, left + 1,left+1, right, i, i, PointerEnum.INDEX_TEXT);
            if (arr[i] < v) {
                Debug.Log(string.Format("swap {0} and {1}", partition, i));
                indexInRound++;
                partition++;
                swap(arr, partition, i);
                createElementAnims(partition, i, round);
            } else {
                createElementAnims(-1, -1, round);
            }

        }
        if (left != partition) {
            indexInRound++;
            swap(arr, left, partition);
            createElementAnims(left, partition, round);
        } 
        return partition;
    }

    private void swap(int[] arr, int i, int j) {
        int t = arr[i];
        arr[i] = arr[j];
        arr[j] = t;

    }

}
