
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSort2: BaseSort {

    enum PointerEnum {
        I_POINTER,
        N_POINTER
    }

    public GameObject iPointerParent;
    public GameObject NPointerParent;
    private PointerInfo iPointerInfo;
    private PointerInfo nPointerInfo;
    // Use this for initialization

    // Update is called once per frame
    void Update() {

    }

    public override void Start() {
        base.Start();
        inputFieldText.transform.parent.GetComponent<InputField>().text = "2,1,3,4,5,6,7";
    }

    public override void dataDropDownChange(int i) {
        base.dataDropDownChange(i);
    }

    private PointerInfo getPointerInfo(PointerEnum pointer) {
        PointerInfo info = new PointerInfo();
        switch (pointer) {
            case PointerEnum.I_POINTER:
                info.goParent = iPointerParent;
                info.baseName = "Index";
                info.rotation = 180;
                info.yOffset = -150;
                break;
            case PointerEnum.N_POINTER:
                info.goParent = NPointerParent;
                info.baseName = "End";
                info.yOffset = 180;
                break;
        }
        return info;
    }

    protected override void restart() {
        base.restart();

        for (int i=0;i< iPointerParent.transform.childCount;i++) {
            Destroy(iPointerParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < NPointerParent.transform.childCount; i++) {
            Destroy(NPointerParent.transform.GetChild(i).gameObject);
        }

        iPointerInfo.goDic.Clear();
        iPointerInfo.goDicDic.Clear();
        iPointerInfo.previousOneDic.Clear();

    }

    protected override void sort(int[] arr) {

        int n = arr.Length;
        int newn; // 使用newn进行优化
        int round = 1;
        iPointerInfo = getPointerInfo(PointerEnum.I_POINTER);
        nPointerInfo = getPointerInfo(PointerEnum.N_POINTER);
        do {
            newStepPointerGo(round, 1, iPointerInfo);
            newRoundPointerGo(round, n-1, nPointerInfo);
            createRoundPointerGoAnim(round,nPointerInfo);
            newn = 0;
            for (int i = 1; i < n; i++) {
                createStepPointerGoAnim(round, 1, 1, n - 1, i, i, iPointerInfo);
                if (arr[i - 1] > arr[i]) {
                    swap(arr, i - 1, i,round);
                    //比如2，1，3，4，5，交换了0，1，newn记录为1
                    //也利用了相邻排序产生的价值信息
                    // 记录最后一次的交换位置,在此之后的元素在下一轮扫描中均不考虑
                    newn = i;
                    createElementAnims(i-1, i, round);
                } else {
                    createElementAnims(-1, -1, round);
                }
            }
            // 优化, 每一趟Bubble Sort都将最大的元素放在了最后的位置
            // 所以下一次排序, 最后的元素可以不再考虑
            n = newn;
            round++;
        } while (newn>0);
    }


}
