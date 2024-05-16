using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextStep : MonoBehaviour
{
    public TextMeshPro textStep;

    //bool是要画什么颜色的字 true白色 false黑色
    public void InitText(int _step, bool color, Vector3 position, float scale)
    {
        textStep.text = _step.ToString();
        textStep.color = color == true ? Color.white : Color.black;
        this.transform.position = position;
        this.transform.localScale = new Vector3(scale, scale, 1);
    }
}
