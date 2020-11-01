using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI DescriptionText;

    RectTransform m_RectTransform;
    //CanvasScaler m_CanvasScaler;
    //Canvas m_Canvas;

    void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        //m_CanvasScaler = GetComponentInParent<CanvasScaler>();
        //m_Canvas = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        m_RectTransform.GetWorldCorners(corners);

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        if (width + 20 < Screen.width - mousePosition.x)
        {
            m_RectTransform.transform.position = mousePosition + Vector3.right * 20;          
        }
        else
        {
            m_RectTransform.transform.position = mousePosition + Vector3.left * (width + 20);
        }
    }
}
