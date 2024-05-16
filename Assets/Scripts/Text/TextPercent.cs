using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPercent : MonoBehaviour
{
    public TextMeshPro textPercent;

    public void InitText(double value, Vector3 position, float scale)
    {
        textPercent.text = Mathf.RoundToInt((float)value).ToString();
        this.transform.position = position;
        this.transform.localScale = new Vector3(scale, scale, 1);
    }
}
