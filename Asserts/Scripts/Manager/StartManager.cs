using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    private bool isIniFinish = false;
    private static List<FoodData> _foodData = new List<FoodData>();

    public AudioSource StartAudioSource;
    //private AdManager AdManager;
    public Transform NoviceTutorial;

    public Transform[] NTS;
    private int NowNTNumber;
    private float LastTime;

    private bool IsReadNT;
    public Transform FavoriteUI;
    private FavoriteFoodUI[] FavoriteFoods = new FavoriteFoodUI[18];
    private FavoriteFoodUI[] FavoriteDeFoods = new FavoriteFoodUI[9];

    private void Awake()
    {
        if (PlayerPrefs.GetInt("IsReadNT") == 0)
        {
            IsReadNT = false;
        }
        else
        {
            IsReadNT = true;
        }
        NowNTNumber = 0;
        LastTime = Time.time;
        NTS = new Transform[12];
        NoviceTutorial.gameObject.SetActive(true);
        for (int i = 0; i < 12; i++)
        {
            NTS[i] = NoviceTutorial.Find("NT" + (i+1).ToString());
        }
        NoviceTutorial.gameObject.SetActive(false);
        Food.IniId();
        GetFavoriteFoods();
        //add by zhujian
        //PlayerPrefs.DeleteAll();

    }
    // Start is called before the first frame update
    void Start()
    {
        if (isIniFinish==false)
        {
            isIniFinish = IniGame();
        }
        



    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnHelpButtonClick()
    {
        NowNTNumber = 0;
        NoviceTutorial.gameObject.SetActive(true);
    }


    public void OnFavoriteButtonClick()
    {
        for (int i = 0; i < 18; i++)
        {
            FavoriteFoods[i].Show();
        }
        for (int i = 0; i < 9; i++)
        {
            FavoriteDeFoods[i].Show();
        }

        FavoriteUI.gameObject.SetActive(true);
    }

    public void OnFavoriteCloseButtonClick()
    {
        FavoriteUI.gameObject.SetActive(false);
        for (int i = 0; i < 18; i++)
        {
            FavoriteFoods[i].Close();
        }
        for (int i = 0; i < 9; i++)
        {
            FavoriteDeFoods[i].Close();
        }  
    }


    private void GetFavoriteFoods()
    {
        for (int i = 0; i < 18; i++)
        {
            string fname;
            if (i < 9)
            {
                fname = "0" + (i + 1);
            }
            else
            {
                fname = (i + 1).ToString();
            }
            FavoriteFoods[i] = FavoriteUI.Find("Scroll View/Viewport/Content/bg/foodUI/" + fname).GetComponent<FavoriteFoodUI>();
        }
        for (int i = 0; i < 9; i++)
        {
            string fname;
             fname = (i+22).ToString();
            FavoriteDeFoods[i] = FavoriteUI.Find("Scroll View/Viewport/Content/bg/defoodUI/" + fname).GetComponent<FavoriteFoodUI>();
        }
    }

    //控制新手教程
    public void OnNTButtonClick()
    {
        if(Time.time - LastTime < 1)
        {
            return;
        }
        else
        {
            LastTime = Time.time;
            if (NowNTNumber==11)
            {
                NoviceTutorial.gameObject.SetActive(false);
                if (!IsReadNT)
                {
                    StartGame();
                }
            }
            else if (IsReadNT && NowNTNumber == 10)
            {
                NoviceTutorial.gameObject.SetActive(false);
                for (int i = 0; i < 12; i++)
                {
                    NTS[i].gameObject.SetActive(true);
                }

            }
            else
            {
                NTS[NowNTNumber].gameObject.SetActive(false);
                NowNTNumber++; 
            }
        }
    } 
    //点击开始游戏
    public void OnStartButtonClick()
    {
        if (isIniFinish)
        {
            StartAudioSource.Play();
            if (IsReadNT)
            {
                Invoke("StartGame", 0.6f);
            }
            else
            {
                
                NoviceTutorial.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("is not Initialize Game");
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Menu");
    }
    public static FoodData GetFoodData(int id)
    {
        foreach (FoodData foodData in _foodData)
        {
            FoodData data = foodData.ReadData(id);
            if (data!=null)
            {
                return data;
            }
        }

        return null;
    }
    /// <summary>
    /// 初始化游戏
    /// </summary>
    /// <returns></returns>
    private bool IniGame()
    {

       return ReadFoodJson();
    }
    /// <summary>
    /// 读取食物属性json文件
    /// </summary>
    /// <returns></returns>
    private bool ReadFoodJson()
    {
        //获取Json中的文本。文本在unity中是TextAsset类型
        TextAsset foodText = Resources.Load<TextAsset>("DataAssets/FoodData");
        if (foodText == null)
        {
            return false;
        }
        //把json文本转换为JsonData格式
        JsonData foodData = JsonMapper.ToObject(foodText.text);
        //对每一个食物，都新建个FoodData类来存储
        for (int i = 0; i < foodData.Count; i++)
        {
            int id = (int)foodData[i]["Id"];
            string name = (string)foodData[i]["Name"];
            float perfectTime = (float)(double)foodData[i]["PerfectTime"];//烹饪到正常可食用状态所需时间，单位秒
            float pasteTime = (float)(double)foodData[i]["PasteTime"]; //烹饪到过火状态所需时间，单位秒
            float failureTime = (float)(double)foodData[i]["FailureTime"];//烹饪到食材消失状态所需时间，单位秒
            int firePowerBase = (int)foodData[i]["FirePowerBase"];//吃掉菜后提供的基础火力值
            float eatPowerBase = (float)(double)foodData[i]["EatPowerBase"];//吃掉菜后提供的基础食力值
            int scoreBase = (int)foodData[i]["ScoreBase"];//食物的基础分数
            FoodData data = new FoodData(id, name, perfectTime, pasteTime,failureTime,firePowerBase,eatPowerBase,scoreBase);
            _foodData.Add(data);
        }
        Debug.Log("_foodData.Count:" + _foodData.Count);
        return true;

    }
}
