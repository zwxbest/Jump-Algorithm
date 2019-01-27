//using DG.Tweening;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class QuickSort1 : BaseSort {
//    enum PointerEnum {
//        INDEX_TEXT,
//        PARTITION_VALUE,
//        PARTITION_POS
//    };

//    public GameObject partitionValueParent;

//    public GameObject partitionPosParent;

//    //value的dic
//    private Dictionary<int, GameObject> partitionValueDic = new Dictionary<int, GameObject>();
//    //partition的dic
//    private Dictionary<int, GameObject> partitionDic = new Dictionary<int, GameObject>();

//    //第一个key为round，第二个key为全局的index
//    private Dictionary<int, Dictionary<int, GameObject>> indexTextDicDic = new Dictionary<int, Dictionary<int, GameObject>>();
//    //第一个key为round，第二个key为全局的index
//    private Dictionary<int, Dictionary<int, GameObject>> partitionPosDicDic = new Dictionary<int, Dictionary<int, GameObject>>();
//    //第一个key为round，第二个key为全局的index
//    private Dictionary<int, Dictionary<int, GameObject>> partitionValueDicDic = new Dictionary<int, Dictionary<int, GameObject>>();

//    void Update() {
       
//    }

//    private void sort(int[] arr) {
//        quickSort(arr, 0, arr.Length - 1, 1);
//    }

//    public void quickSort(int[] arr, int l, int r, int round) {
//        if (l >= r) {
//            return;
//        }
//        int p = partition(arr, l, r, round);
//        quickSort(arr, l, p - 1, round + 1);
//        quickSort(arr, p + 1, r, round + 1);
//    }

//    private PointerInfo getPointerInfo(PointerEnum pointer) {
//        PointerInfo info = new PointerInfo();
//        switch (pointer) {
//            case PointerEnum.INDEX_TEXT:
//                info.goParent = indexTextParent;
//                info.yOffset = -120;
//                info.goDicDic = indexTextDicDic;
//                info.baseName = "indexText";
//                break;
//            case PointerEnum.PARTITION_VALUE:
//                info.goParent = partitionValueParent;
//                info.yOffset = 240;
//                info.goDicDic = partitionValueDicDic;
//                info.baseName = "parititionValue";
//                break;
//            case PointerEnum.PARTITION_POS:
//                info.goParent = partitionPosParent;
//                info.yOffset = 180;
//                info.goDicDic = partitionPosDicDic;
//                info.baseName = "partitionPos";
//                break;
//        }
//        return info;
//    }



      



//    int partition(int[] arr, int left, int right, int round) {
//        int v = arr[left];
//        int partition = left;
        
//        int indexInRound = 0;
//        var indexText = getPointerGo(round, left+1, getPointerInfo(PointerEnum.INDEX_TEXT));
//        var partitionValue = getPointerGo(round, left, getPointerInfo(PointerEnum.PARTITION_VALUE));
//        var partitionPos = getPointerGo(round, partition, getPointerInfo(PointerEnum.PARTITION_POS));
//        for (int i = left + 1; i <= right; i++, indexInRound++) {
//            createPointerGoAnim(round, left,left+1, right, i-1, left, getPointerInfo(PointerEnum.PARTITION_VALUE));
//            createPointerGoAnim(round, left, left+1,right, i, partition, getPointerInfo(PointerEnum.PARTITION_POS) );
//            createPointerGoAnim(round, left + 1,left+1, right, i, i, getPointerInfo(PointerEnum.INDEX_TEXT));
//            if (arr[i] < v) {
//                indexInRound++;
//                partition++;
//                swap(arr, partition, i);
//                createElementAnims(partition, i, round);
//            } else {
//                createElementAnims(-1, -1, round);
//            }

//        }
//        if (left != partition) {
//            indexInRound++;
//            swap(arr, left, partition);
//            createElementAnims(left, partition, round);
//        } 
//        return partition;
//    }



//}
