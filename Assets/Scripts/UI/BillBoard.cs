using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void Update()
    {
        // �I�u�W�F�N�g�ƃJ�����̕��������킹��
        this.transform.forward = Camera.main.transform.forward;
    }
}
