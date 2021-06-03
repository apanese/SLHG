using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData
{
    public readonly int Id;
    public readonly string Name;//食材名字 
    public float PerfectTime;//烹饪到正常可食用状态所需时间，单位秒
    public float PasteTime; //烹饪到过火状态所需时间，单位秒
    public float FailureTime;//烹饪到食材消失状态所需时间，单位秒
    public readonly int FirePowerBase;//吃掉菜后提供的基础火力值
    public readonly float EatPowerBase;//吃掉菜后提供的基础食力值
    public readonly int ScoreBase;//食物的基础分数
    private bool ischange=false;

    public void Show()
    {
        Debug.Log(Id);
        Debug.Log(Name);
        Debug.Log(PerfectTime);
        Debug.Log(PasteTime);
        Debug.Log(FailureTime);
        Debug.Log(FirePowerBase);
        Debug.Log(EatPowerBase);
        Debug.Log(ScoreBase);
    }

    public FoodData(FoodData foodData)
    {

        Id = foodData.Id;
        Name = foodData.Name;
        PerfectTime = foodData.PerfectTime;
        PasteTime = foodData.PasteTime;
        FailureTime = foodData.FailureTime;
        FirePowerBase = foodData.FirePowerBase;
        EatPowerBase = foodData.EatPowerBase;
        ScoreBase = foodData.ScoreBase;
        
    }

    public FoodData(int id, string name, float perfectTime, float pasteTime, float failureTime, int firePowerBase, float eatPowerBase, int scoreBase)
    {
        Id = id;
        Name = name;
        PerfectTime = perfectTime;
        PasteTime = pasteTime;
        FailureTime = failureTime;
        FirePowerBase = firePowerBase;
        EatPowerBase = eatPowerBase;
        ScoreBase = scoreBase;
    }
    //根据id读取数据，若为没有返回空
    public FoodData ReadData(int id)
    {
        if (this.Id==id)
        {
            return this;
        }
        else
        {
            return null;
        }
    }
    public void ChangeData()
    {
        if (!ischange)
        {
            this.PerfectTime = this.PerfectTime / 2;
            this.PasteTime = this.PasteTime / 2;
            this.FailureTime = this.FailureTime / 2;
            ischange = true;
        }
    }
    public void RecoveryData()
    {
        this.PerfectTime = this.PerfectTime * 2;
        this.PasteTime = this.PasteTime * 2;
        this.FailureTime = this.FailureTime * 2;
        ischange = false;
    }
}
