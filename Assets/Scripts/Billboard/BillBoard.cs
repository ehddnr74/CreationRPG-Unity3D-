using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform nameTagTr;

    // Update is called once per frame
    void Update()
    {
        nameTagTr.position = nameTagTr.position;
        nameTagTr.rotation = Quaternion.LookRotation(-(nameTagTr.position - Camera.main.transform.position));
    }
}
