using UnityEngine;
using System.Collections;
using UnityEditor;

/************************************************************
  FileName: AnchorsAdapt.cs
  Author:菜菜       Version :1.0          Date: 2019.3.7
  Description:放在Editor文件夹中的编辑器的工具类，用于GUI画布内组件。让锚点快速定位到组件的四个顶点
************************************************************/

public class AnchorsAdapt
{
    [MenuItem("Tools/AnchorsAdapt")]
    private static void SelectionM()
    {
        GameObject[] gos = Selection.gameObjects;
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].GetComponent<RectTransform>() == null)
                continue;
            Adapt(gos[i]);
        }
    }

    private static void Adapt(GameObject go)
    {
        //位置信息
        Vector3 partentPos = go.transform.parent.position;
        Vector3 localPos = go.transform.position;
        //------获取rectTransform----
        RectTransform partentRect = go.transform.parent.GetComponent<RectTransform>();
        RectTransform localRect = go.GetComponent<RectTransform>();
        float partentWidth = partentRect.rect.width;
        float partentHeight = partentRect.rect.height;
        float localWidth = localRect.rect.width * 0.5f;
        float localHeight = localRect.rect.height * 0.5f;
        //---------位移差------
        float offX = localPos.x - partentPos.x;
        float offY = localPos.y - partentPos.y;

        float rateW = offX / partentWidth;
        float rateH = offY / partentHeight;
        localRect.anchorMax = localRect.anchorMin = new Vector2(0.5f + rateW, 0.5f + rateH);
        localRect.anchoredPosition = Vector2.zero;

        partentHeight = partentHeight * 0.5f;
        partentWidth = partentWidth * 0.5f;
        float rateX = (localWidth / partentWidth) * 0.5f;
        float rateY = (localHeight / partentHeight) * 0.5f;
        localRect.anchorMax = new Vector2(localRect.anchorMax.x + rateX, localRect.anchorMax.y + rateY);
        localRect.anchorMin = new Vector2(localRect.anchorMin.x - rateX, localRect.anchorMin.y - rateY);
        localRect.offsetMax = localRect.offsetMin = Vector2.zero;
    }
}
