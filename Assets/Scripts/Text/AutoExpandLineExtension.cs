using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TextMeshPro有这个功能
public class AutoExpandLineExtension : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text txt=this.GetComponent<Text>();
        // 创建父对象
        GameObject dividerParent = new GameObject("Divider");
        RectTransform dividerRect = dividerParent.AddComponent<RectTransform>();
        dividerRect.SetParent(this.transform, false);

        // 设置父对象的位置和锚点与文本对象相同
        dividerRect.position = txt.rectTransform.position;
        dividerRect.anchorMin = new Vector2(0, 0.5f);
        dividerRect.anchorMax = new Vector2(0, 0.5f);
        dividerRect.pivot = new Vector2(0, 0.5f);

        // 添加Image组件并设置颜色
        Image dividerImage = dividerParent.AddComponent<Image>();
        dividerImage.color = Color.white;

        // 设置父对象的大小与文本对象的大小相同
        dividerRect.sizeDelta = new Vector2(txt.fontSize * txt.text.Length, 1);

        // 设置父对象的位置在文本对象的下方
        dividerRect.anchoredPosition = new Vector2(0, -txt.fontSize + 5);
    }
}
