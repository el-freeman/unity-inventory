using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    public static DamageUI Instance { get; private set; }

    public class ActiveText
    {
        public Text UIText;
        public float MaxTime;
        public float Timer;
        public Vector3 WorldPositionStart;       

        public void PlaceText(Camera cam, Canvas canvas)
        {
            float ratio = 1.0f - (Timer / MaxTime);
            Vector3 pos = WorldPositionStart + new Vector3(ratio, Mathf.Sin(ratio * Mathf.PI), 0);
            pos = cam.WorldToScreenPoint(pos) + new Vector3(25, 5, 0);
            //pos *= canvas.scaleFactor;
            pos.z = 0.0f;

            UIText.transform.position = pos;
        }
    }

    public Text DamageTextPrefab;

    Canvas m_Canvas;
    Queue<Text> m_TextPool = new Queue<Text>();
    List<ActiveText> m_ActiveTexts = new List<ActiveText>();

    Camera m_MainCamera;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_Canvas = GetComponent<Canvas>();

        const int POOL_SIZE = 64;
        for (int i = 0; i < POOL_SIZE; ++i)
        {
            var t = Instantiate(DamageTextPrefab, m_Canvas.transform);
            t.gameObject.SetActive(false);
            m_TextPool.Enqueue(t);
        }

        m_MainCamera = Camera.main;
    }

    void Update()
    {
        for (int i = 0; i < m_ActiveTexts.Count; ++i)
        {
            var at = m_ActiveTexts[i];
            at.Timer -= Time.deltaTime;

            if (at.Timer <= 0.0f)
            {
                at.UIText.gameObject.SetActive(false);
                m_TextPool.Enqueue(at.UIText);
                m_ActiveTexts.RemoveAt(i);
                i--;
            }
            else
            {
                var color = at.UIText.color;
                color.a = at.Timer / at.MaxTime;
                at.UIText.color = color;
                at.PlaceText(m_MainCamera, m_Canvas);
            }
        }
    }

    public void NewDamage(int amount, Vector3 worldPos)
    {
        var t = m_TextPool.Dequeue();

        t.text = amount.ToString();
        t.gameObject.SetActive(true);

        ActiveText at = new ActiveText();
        at.MaxTime = 1.0f;
        at.Timer = at.MaxTime;
        at.UIText = t;
        at.WorldPositionStart = worldPos + Vector3.up;       

        at.PlaceText(m_MainCamera, m_Canvas);

        m_ActiveTexts.Add(at);
    }
}
