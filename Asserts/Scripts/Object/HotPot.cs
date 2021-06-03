using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotPot : MonoBehaviour
{
    private static GameObject[] CookFood = new GameObject[4];
    private static Transform[] Pos = new Transform[4];
    private static Transform[] PerfectTips = new Transform[4];
    public Transform HotPotUI;
    public Animator animator;

    private static HotPot _HotPot;

    public Text SoupBaseTestUI;

    private static float SoupBaseValue;//当前汤底值

    private static float yMax;
    private static float yMin;
    private static float xMin;
    private static float xMax;
    private static float xCenter;
    private static float yCenter;
    private void Awake()
    {
        _HotPot = this.GetComponent<HotPot>();
        SoupBaseValue = 100;//当前汤底值
        //animator = transform.GetComponent<Animator>();
        Pos[0] = GameObject.Find("Canvas_Game/Background/HotPot/P1").transform;
        Pos[1] = GameObject.Find("Canvas_Game/Background/HotPot/P2").transform;
        Pos[2] = GameObject.Find("Canvas_Game/Background/HotPot/P3").transform;
        Pos[3] = GameObject.Find("Canvas_Game/Background/HotPot/P4").transform;
        PerfectTips[0] = GameObject.Find("Canvas_Game/Background/HotPot/P1/Perfect").transform;
        PerfectTips[1] = GameObject.Find("Canvas_Game/Background/HotPot/P2/Perfect").transform;
        PerfectTips[2] = GameObject.Find("Canvas_Game/Background/HotPot/P3/Perfect").transform;
        PerfectTips[3] = GameObject.Find("Canvas_Game/Background/HotPot/P4/Perfect").transform;
        SetHotPotPos();

    }
    private void Update()
    {
        SoupBaseTestUI.text = SoupBaseValue.ToString("f2");
        //_HotPot.ChangeState();
    }
    //减少汤底
    public static void DecSoupBase(float value)
    {
        SoupBaseValue -= value;
    }
    //判断坐标所属位置
    public static int IsTargetLocation(Vector3 vector)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(vector);
        int loc = 0;
        if (pos.x>xCenter)
        {
            loc += 1;
        }

        if (pos.y<yCenter)
        {
            loc += 2;
        }

        return loc;
    }

    //判断坐标是否在火锅内
    public static bool IsInHotPot(Vector3 vector)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(vector);
       // Debug.Log(xMin+"<"+pos.x+ "<"+xMax+ ","+yMin+ "<"+pos.y+ "<"+yMax);
        if (pos.x > xMin && pos.x < xMax)
        {
            if (pos.y > yMin && pos.y < yMax)
            {
                return true;
            }
        }
        return false;
    }

    //初始化火锅判定范围和中心坐标
    private void SetHotPotPos()
    {
        Vector3[] corners = new Vector3[4];
        HotPotUI.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
        int n = 0;
        foreach (var c in corners)
        {
            Vector3 item = Camera.main.WorldToScreenPoint(c);
            if (n == 0)
            {
                xMin = item.x;
                yMin = item.y;
            }

            if (n == 2)
            {
                xMax = item.x;
                yMax = item.y;
            }
            n++;
        }

        xCenter = Camera.main.WorldToScreenPoint(HotPotUI.position).x;
        yCenter = Camera.main.WorldToScreenPoint(HotPotUI.position).y;
    }

    //出锅
    public static void DeleteFood(int index ,bool isFailed = false)
    {
        if (!isFailed)
        {
            Food food = CookFood[index].GetComponent<Food>();
            //是否红汤
            if (index % 2 == 0)
            {
                food.FirePowerValue *= 3;
                food.IsFire = true;
            }
            //计算汤底对食力值改变
            //if (SoupBaseValue < 20)
            //{
            //    food.EatPowerValue = -food.EatPowerValue;
            //}
            //else if (SoupBaseValue < 60)
            //{
            //    food.EatPowerValue = 0;
            //}
            //else if (SoupBaseValue < 80)
            //{
            //    food.EatPowerValue /= 2;
            //}
        }
        CookFood[index] = null;
    }
    //计算汤底状态
    //private void ChangeState()
    //{
    //    if (SoupBaseValue < 20)
    //    {
    //        animator.SetInteger("State", 3);
    //    }
    //    else if (SoupBaseValue < 60)
    //    {
    //        animator.SetInteger("State", 2);
    //    }
    //    else if (SoupBaseValue < 80)
    //    {
    //        animator.SetInteger("State", 1);
    //    }    
    //}

    public static bool CheckPosNull(int index) => CookFood[index] == null;

    //显示成熟    隐藏成熟
    public static void SetPerfectTips(int index,bool isShow)
    {
        PerfectTips[index].GetComponent<Animator>().SetBool("IsShow", isShow);
    }

    /// <summary>
    /// 把食物放到锅里
    /// </summary>
    /// <param name="food"></param>
    /// <param name="index"></param>
    public static void AddInHotPot(GameObject food,int index)
    {
        food.transform.parent = _HotPot.GetComponent<Transform>();
        CookFood[index] = food;
        food.GetComponent<Food>().NowPos = index;
    }

    public static Vector3 GetCookPos(int index) => Pos[index].position;
}
