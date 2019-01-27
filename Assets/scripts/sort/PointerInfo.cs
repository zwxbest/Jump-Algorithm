using System.Collections.Generic;
using UnityEngine;

public class PointerInfo {
    public GameObject goParent { get; set; }
    public int yOffset { get; set; }
    public string baseName { get; set; }

    public float rotation { get; set; }

    //第一个int表示round，第二个int表示index
    public Dictionary<int, Dictionary<int, GameObject>> goDicDic { get; }

    public Dictionary<GameObject, GameObject> previousOneDic { get; }

    public Dictionary<int,GameObject> goDic { get; }

    public PointerInfo() {
        goDicDic = new Dictionary<int, Dictionary<int, GameObject>>();
        goDic = new Dictionary<int, GameObject>();
        previousOneDic = new Dictionary<GameObject, GameObject>();
    }
}