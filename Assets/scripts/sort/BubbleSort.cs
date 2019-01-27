
using DG.Tweening;
using UnityEngine;

public class BubbleSort : BaseSort {

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
        bool swapped = false;
        int round = 1;
        iPointerInfo = getPointerInfo(PointerEnum.I_POINTER);
        nPointerInfo = getPointerInfo(PointerEnum.N_POINTER);
        do {
            newStepPointerGo(round, 1, iPointerInfo);
            newRoundPointerGo(round, n-1, nPointerInfo);
            createRoundPointerGoAnim(round,nPointerInfo);
            swapped = false;
            for (int i = 1; i < n; i++) {
                createStepPointerGoAnim(round, 1, 1, n - 1, i, i, iPointerInfo);
                if (arr[i - 1] > arr[i]) {
                    swap(arr, i - 1, i,round);
                    swapped = true;
                    createElementAnims(i-1, i, round);
                } else {
                    createElementAnims(-1, -1, round);
                }
            }
            // 优化, 每一趟Bubble Sort都将最大的元素放在了最后的位置
            // 所以下一次排序, 最后的元素可以不再考虑
            n--;
            round++;
        } while (swapped);
    }


}
