
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 插入排序
/// </summary>
public class InsertionSort2 : BaseSort {

    enum PointerEnum {
        I_POINTER,
        J_POINTER
    }

    public override void Start() {
        base.Start();
      
    }


    public GameObject I_PointerParent;
    public GameObject J_PointerParent;
    private PointerInfo iPointerInfo;
    private PointerInfo jPointerInfo;
    // Use this for initialization

    // Update is called once per frame
    void Update() {

    }

    private PointerInfo getPointerInfo(PointerEnum pointer) {
        PointerInfo info = new PointerInfo();
        switch (pointer) {
            case PointerEnum.I_POINTER:
                info.goParent = I_PointerParent;
                info.baseName = "I";
                info.rotation = 180;
                info.yOffset = -200;
                break;
            case PointerEnum.J_POINTER:
                info.goParent = J_PointerParent;
                info.baseName = "J";
                info.yOffset = 180;
                break;
        }
        return info;
    }

    protected override void restart() {
        base.restart();

        for (int i = 0; i < I_PointerParent.transform.childCount; i++) {
            Destroy(I_PointerParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < J_PointerParent.transform.childCount; i++) {
            Destroy(J_PointerParent.transform.GetChild(i).gameObject);
        }

    }

    protected override void sort(int[] arr) {
        int n = arr.Length;
        int round = 0;

        PointerInfo iPointerInfo = getPointerInfo(PointerEnum.I_POINTER);
        PointerInfo jPointerInfo = getPointerInfo(PointerEnum.J_POINTER);

        for (int i = 0; i < n; i++) {
            round++;
            newRoundPointerGo(round, i, iPointerInfo);
            createRoundPointerGoAnim(round, iPointerInfo);
            newStepPointerGo(round, i, jPointerInfo);
            for (int j = i; j > 0 && arr[j] < arr[j - 1] ; j--) {
                createStepPointerGoAnim(round, i, i, 1, j, j, jPointerInfo);
                swap(arr, j, j - 1,round);
                createElementAnims(j, j - 1, round);
            }
            createElementAnims(-1, -1, round);

       }


    }
}
