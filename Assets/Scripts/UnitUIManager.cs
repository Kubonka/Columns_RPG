using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public class UnitUIManager : MonoBehaviour
{
    public Canvas canvas;
    private GameObject buffs;
    private GameObject debuffs;
    private Vector2 pos;

    private void Awake()
    {
        Debug.Log(pos.x);
        pos = new Vector2(0, 0);
        if (this.gameObject.name == "Enemy")
            pos = new Vector2(480, 0);
    }
    void Start()
    {
        this.gameObject.GetComponent<Unit>().onDestroy += DestroyElement;

        buffs = new GameObject("Buffs", typeof(RectTransform));
        buffs.transform.SetParent(canvas.transform);
        buffs.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300f+pos.x,-150f+pos.y);
        debuffs = new GameObject("Debuffs", typeof(RectTransform));
        debuffs.transform.SetParent(canvas.transform);
        debuffs.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400f+pos.x, -150f+pos.y);
        
    }

    public void Refresh(List<GameObject> buffsList, List<GameObject> debuffsList)
    {
        int buffsCount = buffs.transform.childCount;
        int debuffCount = debuffs.transform.childCount;
        for (int i = 0; i < buffsCount; i++)
        {
            buffs.transform.GetChild(i).GetComponentInChildren<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buffs.transform.GetChild(i).GetComponentInChildren<Text>().text = buffsList[i].GetComponent<Skill>().buffTime.ToString();
            buffs.transform.GetChild(i).GetComponentInChildren<Image>().sprite = buffsList[i].GetComponent<Skill>().skillData.icon;
        }
        for (int i = 0; i < debuffCount; i++)
        {
            debuffs.transform.GetChild(i).GetComponentInChildren<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            debuffs.transform.GetChild(i).GetComponentInChildren<Text>().text = buffsList[i].GetComponent<Skill>().buffTime.ToString();
            debuffs.transform.GetChild(i).GetComponentInChildren<Image>().sprite = buffsList[i].GetComponent<Skill>().skillData.icon;
        }
    }   

    public void CreateElement(bool buff) //buff == true , debuff = false
    {
        if (buff)
        {
            int pos = GetPosition(buffs);
            SetElementValues(pos);
        }
        else
        {
            int pos = GetPosition(debuffs);
            SetElementValues(pos);
        }
    }
    private void SetElementValues(int pos)
    {
        GameObject go = new GameObject("buff/debuff", typeof(RectTransform));
        go.transform.SetParent(buffs.transform);
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos * 30f, 0f);
        GameObject go1 = new GameObject("Text", typeof(RectTransform));
        go1.transform.SetParent(go.transform);
        go1.AddComponent<Text>();
        go1.layer = 5;
        go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -20f);
        go1.GetComponent<RectTransform>().GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        GameObject go2 = new GameObject("Icon", typeof(RectTransform));
        go2.transform.SetParent(go.transform);
        go2.AddComponent<Image>();
        go2.layer = 5;
        go2.GetComponent<Image>().transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        go2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        Debug.Log(go2.GetComponent<Image>().transform.localScale);
    }
    private int GetPosition(GameObject element)
    {
        int pos = element.transform.childCount;
        return pos;
    }

    public void DestroyElement(bool buff) //buff == true , debuff = false
    {
        int lastPos;
        if (buff)
        {            
            lastPos = canvas.transform.Find("Buffs").childCount - 1;
            //Debug.Log("LASTPOS= "+lastPos);
            DestroyImmediate(canvas.transform.Find("Buffs").GetChild(lastPos).gameObject);
        }
        else
        {
            lastPos = canvas.transform.Find("Buffs").childCount - 1;
            //Debug.Log("LASTPOS= " + lastPos);
            DestroyImmediate(canvas.transform.Find("Debuffs").GetChild(lastPos).gameObject);
        }
    }

    //private void OnDisable()
    //{
    //    GetComponent<PlayerManager>().onDestroy -= DestroyElement;
    //}
}
