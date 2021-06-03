using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    private int Id;
    private static readonly int FristId = 1;
    private static int NextId;
    private int FoodId;
    private string Name;//食材名字
    private float CookTime;//已用烹饪时间，单位秒
    private float PerfectTime;//烹饪到正常可食用状态所需时间，单位秒
    private float PasteTime; //烹饪到过火状态所需时间，单位秒
    private float FailureTime;//烹饪到食材消失状态所需时间，单位秒

    private enum FoodState//食物所处于的状态
    {
        Raw,//生的
        Perfect,//完美
        Paste,//过火
        Failing//失败
    }

    public bool isCooking;
    private FoodState CookState;
    private int FirePowerBase;//吃掉菜后提供的基础火力值
    private float EatPowerBase;//吃掉菜后提供的基础食力值

    public bool IsFire;

    public int FirePowerValue;//吃掉菜后提供的实际火力值
    public float EatPowerValue;//吃掉菜后提供的实际食力值


    private int ScoreBase;//食物的基础分数
    public int ScoreValue;//食物的基础分数


    private Animator animator;

    private bool IsTouch=false;
    private bool IsInDish=false;

    private bool isPerfectTipsShow;
    public int NowPos;

    private bool isMouseDown = false;

    private Vector3 lastMousePosition = Vector3.zero;

    private int FlickerTimes;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }
    private void Update()
    {       
        UpDateState();
        UpDateCookTime();
        if (!IsInDish)
        {
            UpdateMove();
        }
        
    }


    //被拖动
    void UpdateMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            lastMousePosition = Vector3.zero;
        }
        if (isMouseDown && IsTouch)
        {
            if (CookTime > 0) { PromptPerfect(false); }
            if (lastMousePosition != Vector3.zero)
            {
                //Camera.main.ScreenToWorldPoint(Input.mousePosition)
                //print (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                
                Vector3 offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;


                transform.position = transform.position + offset;
                checkPosition();
            }
            lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void checkPosition()
    {
        //check y -3.3f +3.3f
        //check x -1.9f +1.9f
        Vector3 pos = transform.position;
        float x = pos.x;
        float y = pos.y;
        /*if (y < -3.3f)
        {
            y = -3.3f;
        }*/
        if (y > 3.7f)
        {
            y = 3.7f;
        }
        /*if (x < -1.9f)
        {
            x = -1.9f;
        }
        if (x > 1.9f)
        {
            x = 1.9f;
        }*/
        transform.position = new Vector3(x, y, 0);
    }

    public void CalculateValue()
    {
        //食力值
        //火力值
        //分数
        if (CookState == FoodState.Raw)
        {
            EatPowerValue -= (EatPowerBase * 2);
            FirePowerValue = 0;
            ScoreValue = (int)(ScoreValue*0.1);
        }
        else if (CookState == FoodState.Paste)
        {
            EatPowerValue -= (float)(EatPowerBase * 0.5);
            FirePowerValue -= (int)(FirePowerValue * 0.5);
            ScoreValue = (int)(ScoreValue * 0.5);
        }

    }
    public void Eat()
    {
        Destroy(transform.gameObject);
    }

    private void DeleteFailingFood()
    {
        if (CookTime==0)
        {
            return;
        }

        GameManager._gameManager.InterruptCombo();
        //HotPot.DecSoupBase(EatPowerBase/4.0f);
        HotPot.DeleteFood(NowPos,true);

        Destroy(transform.gameObject);
    }

    public void InDish()
    {
        IsInDish = true;
        PromptPerfect(false);
    }

    public void OnCook()
    {
        isCooking = true;

    }

    public void OnDish()
    {
        isCooking = false;
        animator.SetBool("IsInDish", true);
    }
    public void OnTouch()
    {
        
        IsTouch = true;
        animator.SetBool("IsTouch", true);
        isCooking = false;
    }
    public void OffTouch()
    {
        IsTouch = false;
        animator.SetBool("IsTouch", false);
    }

    public void Cook(bool isRed = false)
    {
        if (isRed && CookTime == 0)
        {
            FirePowerValue *= 3;
        }
        isCooking = true;
        //animator.SetBool("IsTouch", false);
    }


    private void UpDateCookTime()
    {
        if (isCooking)
        {
            CookTime += (Time.deltaTime * GameManager.GetCookSpeed());
        }
    }

    /// <summary>
    /// 更新食物烹饪状态
    /// </summary>
    private void UpDateState()
    {
        if (CookTime < PerfectTime)
        {
            CookState = FoodState.Raw;
            animator.SetInteger("CookState", CookTime == 0 ? 0 : 1);
        }
        else if (CookTime < PasteTime)
        {
            CookState = FoodState.Perfect;
            animator.SetInteger("CookState", 2);
            if (!isPerfectTipsShow)
            { 
                PromptPerfect(true);
                isPerfectTipsShow = true;
            }
        }
        else if (CookTime < FailureTime)
        {
            CookState = FoodState.Paste;
            animator.SetInteger("CookState", 3);
            if (!IsInDish)
            {
                PromptPerfect(false);
            }
        }
        else
        {
            CookState = FoodState.Failing;
            animator.SetInteger("CookState", 4);
            Invoke("DeleteFailingFood", 0.9f);
        }
    }

    
    //成熟提示
    private void PromptPerfect(bool isShow)
    {
            HotPot.SetPerfectTips(NowPos, isShow);
    }
    //初始化ID系统
    //自动写入ID
    public static void IniId()
    {
        NextId = FristId;
        NextId++;
    }
    /// <summary>
    /// 读取data数据构造
    /// </summary>
    /// <param name="foodData"></param>
    public void SetFood(FoodData foodData)
    {
        this.Id = NextId;
        NextId++;
        FoodId = foodData.Id;
        CookState = FoodState.Raw;
        isCooking = false;
        Name = name;
        CookTime = 0;
        IsFire = false;
        PerfectTime = foodData.PerfectTime;
        PasteTime = foodData.PasteTime;
        FailureTime = foodData.FailureTime;
        FirePowerBase = foodData.FirePowerBase;
        EatPowerBase = foodData.EatPowerBase;
        FirePowerValue = FirePowerBase;
        EatPowerValue = EatPowerBase;
        ScoreBase = foodData.ScoreBase;
        ScoreValue = ScoreBase;
        NowPos = 5;
        isPerfectTipsShow = false;
    }
    //是否完美
    public bool IsPerfect() => CookState == FoodState.Perfect;
    //是否在盘子内
    public bool GetIsInDish() => IsInDish;
    //获取烹饪时间
    public float GetCookTime() => CookTime;
    //获取被点击状态
    public bool GetTouch() => IsTouch;
    //获取ID
    public int GetId() => this.Id;
    //获取食物ID
    public int GetFoodId() => this.FoodId;
}
