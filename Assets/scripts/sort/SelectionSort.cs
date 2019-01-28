
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectionSort: BaseSort {

    enum PointerEnum {
        MIN_POINTER,
        J_POINTER
    }

    public GameObject minPointerParent;
    public GameObject jPointerParent;
    private PointerInfo minPointerInfo;
    private PointerInfo jPointerInfo;
    // Use this for initialization

    // Update is called once per frame
    void Update() {

    }

    public override void Start() {
        base.Start();
        inputFieldText.transform.parent.GetComponent<InputField>().text = "7,6,2,3,4,5,1";
    }

    public override void dataDropDownChange(int i) {
        base.dataDropDownChange(i);
    }

    private PointerInfo getPointerInfo(PointerEnum pointer) {
        PointerInfo info = new PointerInfo();
        switch (pointer) {
            case PointerEnum.MIN_POINTER:
                info.goParent = minPointerParent;
                info.baseName = "MIN";
                info.rotation = 180;
                info.yOffset = -200;
                break;
            case PointerEnum.J_POINTER:
                info.goParent = jPointerParent;
                info.baseName = "J";
                info.yOffset = 180;
                break;
        }
        return info;
    }

    protected override void restart() {
        base.restart();

        for (int i=0;i< minPointerParent.transform.childCount;i++) {
            Destroy(minPointerParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < jPointerParent.transform.childCount; i++) {
            Destroy(jPointerParent.transform.GetChild(i).gameObject);
        }

    }

    protected override void sort(int[] arr) {

        int n = arr.Length;
        int round = 0;
        minPointerInfo = getPointerInfo(PointerEnum.MIN_POINTER);
        jPointerInfo = getPointerInfo(PointerEnum.J_POINTER);
        for (int i = 0; i < n; i++) {
            round++;
            // 寻找[i, n)区间里的最小值的索引
            int minIndex = i;
            newRoundPointerGo(round, minIndex, minPointerInfo);
            newStepPointerGo(round, i+1, jPointerInfo);
            createRoundPointerGoAnim(round, minPointerInfo);
            for (int j = i + 1; j < n; j++) {
                //选择排序的价值没有被充分利用
                //使用compareTo方法比较两个Comparable对象的大小
                createStepPointerGoAnim(round, i+1,i + 1, n - 1, j, j, jPointerInfo);
                if (arr[j] < arr[minIndex]) {
                    minIndex = j;
                }
            }
            if (minIndex != i) {
                swap(arr, i, minIndex,round);
                createElementAnims(minIndex, i, round);
            } else {
                createElementAnims(-1, -1, round);
            }
        }
    }

}
