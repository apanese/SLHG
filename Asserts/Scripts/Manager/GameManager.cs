using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public class GameManager : MonoBehaviour
{
    private static readonly List<FoodData> GameFoodData = new List<FoodData>();
    public static GameObject TouchFood;
    private  GameObject Fire;
    private static bool isIniFoodData = false;
    private AdManager adManager;
    private bool isGameOver = false;

    //上菜属性
    private static float ServingSpeed;//为上菜间隔速度
    //*******************************
    //*          玩家属性           *
    //*******************************
    private int Score;//分数
    public float EatPower;//食力值
    private float EatPowerPadValue;//食力衰减值
    //private float FirePowerHurt;//火力对食力的伤害

    //private int FirePower;//火力值
    //private int FirePowerPadValue;//火力衰减值
    //private float FirePowerMultiple;//火力对分数加成倍数

    private static int ComboNumber;//连击数
    private float CookSpeedBase;//烹饪速度基数
    private static float CookSpeed;//烹饪速度倍数

    private float EatTime;//为吃间隔时间
    private static float ComboTime;//为连击间隔时间

    private static float GameTime;//游戏时间

    private int HighScore;
    private int HighCombo;
    private int FoodNumber;

    private int WaterNumber;

    //收藏
    private int[] FavoriteFood = new int[30];

    public Transform FavoriteUI;
    private FavoriteFoodUI[] FavoriteFoods = new FavoriteFoodUI[18];
    private FavoriteFoodUI[] FavoriteDeFoods = new FavoriteFoodUI[9];
    //###############################
    //#          游戏属性           #
    //###############################
    //游戏模式
    private GameMode gameMode;
    //限时模式
    private readonly int TimeModeTime = 180;
    //-------------------------------------
    //||           游戏组件              ||
    //-------------------------------------
    public GameObject LimitedTimeSupply;
    public GameObject EatPowerUI;
    //垃圾桶
    public Animator TrashAnimator;
    public Transform TrashUI;
    //熊猫
    public Transform Panda;
    //文字
    private Text EndTimeText;
    private Text HighComboText;
    private Text HighScoreText;
    private Text FoodNumberText;

    public Transform pauseUI;
    public Transform EndUI;
    
    public Slider EatPowerSlider;
    public Text TimeText;
    public Text ScoreText;

    public Text FireText;
    //分数相关
    public Transform endScore;
    public Transform addScore;
    public Transform ScoreEndPos;

    public Transform ComboNumberView;

    public Animator PandaAnimator;
    //声音
    public AudioSource UISource;
    public AudioSource EatSource;
    public AudioSource BGMSource;
    public AudioSource EndSource;
    public AudioSource DrinkSource1;
    public AudioSource DrinkSource2;
    public AudioSource TrashSource;
    
    //辣椒栏动画
    public Animator FireShowAnimator;
    //水杯动画
    public Animator CupAnimator;

    private Vector3 startPos;
    private Vector3 endPos;

    private Image[] EndScoreImageList;//美术字列表
    private Image[] AddScoreImageList;//美术字列表
    private Image[] ComboImageList;//美术字列表

    //正在吃
    private bool IsEating;
    //临时变量
    public static bool IsMouseOnDish = false;
    public static bool IsMouseOnHotPot = false;
    public static int FoodTargetLocation = 0;
    private float TempScore = 0;

    private float ScoreShowTime;

    private bool IsAddScoreShow;

    private bool IsDrinking;

    //秒，控制按秒数秒执行的程序
    private int Second;

    public static GameManager _gameManager;

    public int FireShowNumber;

    
    public enum GameMode
    {
        Standard,
        Time
    }
    
    //美术字类型(不同的表现)
    enum E_NumType
    {
        Type_1,
        Type_2
    }

    private void Awake()
    {
        if (isIniFoodData)
        {
            Time.timeScale = 1;
            IniGameData();
            IniGameSet();
        }
        else
        {
            SceneManager.LoadScene("Start");
        }
        adManager = GameObject.Find("AdManager").GetComponent<AdManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        IniFavoriteFood();
    }

    // Update is called once per frame
    void Update()
    {
        UpDateTime();
        UpDateScore();
        UpDataValue();
        OnTouch();
        UpdateEatPower();
        UpdateSecond();
        UpdateScoreShow();
        if (!(Dish.CheckNull() || IsEating)) { AutoEat(); }
        //FireText.text = "火力："+ FirePower;
    }
    //++++++++++++++++++++++++++++控制+++++++++++++++++++++++++++++

    //食物点击判断
    private void OnTouch()
    {
        if (isGameOver)
        {
            return;
        }
        if (TouchFood != null)
        {
            IsMouseOnHotPot = HotPot.IsInHotPot(TouchFood.transform.position);
            IsMouseOnDish = Dish.IsInDish(TouchFood.transform.position);
            FoodTargetLocation = HotPot.IsTargetLocation(TouchFood.transform.position);
            TrashAnimator.SetBool("IsChoose", IsInTransform(TouchFood.transform.position, TrashUI));
           //释放TouchFood
                if (TouchFood.GetComponent<Food>().GetTouch() == false)
            {
                TouchFood = null;
            }
        }
        //当前是否有任何鼠标按钮或键被按住
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(new Vector2(v.x, v.y), new Vector2(v.x, v.y), 0.1f); //射线碰撞
            if (Physics2D.Raycast(new Vector2(v.x, v.y), new Vector2(v.x, v.y), 0.1f))
            {
                //Destroy(redpoint);//销毁上一个点
                Debug.DrawLine(new Vector2(v.x, v.y), hitInfo.point); //绘制射线
                if (hitInfo.collider.gameObject.tag == "Food")
                {
                    TouchFood = hitInfo.collider.gameObject;

                    if (!TouchFood.GetComponent<Food>().GetIsInDish())
                    {
                        TouchFood.GetComponent<Food>().OnTouch();
                    }
                    else
                    {
                        TouchFood = null;
                    }
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (TouchFood != null)
            {
                 //是否放入垃圾桶
                if (IsInTransform(TouchFood.transform.position, TrashUI))
                {
                    PutFoodInTrash(TouchFood);
                    TrashAnimator.SetBool("IsChoose", false);
                }
                //是否放入火锅
                else if (IsMouseOnHotPot && TouchFood.GetComponent<Food>().GetCookTime() == 0)
                {
                    CookTouchFood();
                }
                //是否放入盘子
                else if (IsMouseOnDish && TouchFood.GetComponent<Food>().GetCookTime() > 0 && Dish.CheckNull())
                {
                    PutFoodInDish(TouchFood);
                }
               
                else if (TouchFood.GetComponent<Food>().GetCookTime() > 0)
                {
                    ReturnCook();
                }
                if (TouchFood!=null)
                {
                    TouchFood.GetComponent<Food>().OffTouch();
                }
            }
        }


    }

    //---------------------------------Button--------------------------------------
    public void OnDrinkButtonClick()
    {
        if (FireShowNumber==8)
        {
            IsDrinking = true;
            PandaAnimator.SetBool("IsDrink", true);
            Invoke("PlayDrink" + "1" + "Audio", 0.1f);
            
            Invoke("DrinkEnd", 0.9f);
            Invoke("DrinkEnd2", 1.25f);

        }
    }
    public void DrinkEnd()
    {
        PandaAnimator.SetBool("IsDrink", false);
        DrinkWater();
    }
    public void DrinkEnd2()
    {
        IsDrinking = false;
    }
    public void OnEatButtonClick()
    {
        if (!(Dish.CheckNull() || IsEating))
        {
            EatDishFood();
            PlayEatAudio();
            PandaAnimator.SetBool("Eat", true);
            Invoke("EndPandaEat", 0.3f);
        }
    }
    public void AutoEat()
    {
        if (!(Dish.CheckNull() || IsEating||IsDrinking))
        {
            EatDishFood();
            PlayEatAudio();
            PandaAnimator.SetBool("Eat", true);
            Invoke("EndPandaEat", 0.3f);
        }
    }

    private void EndPandaEat()
    {
        PandaAnimator.SetBool("Eat", false);
    }
    public void OnGoStartButtonClick()
    {
        //add by jason
        adManager.ShowFullScreenVideoAd();
        adManager.LoadFullScreenVideoAd();
        PlayUiAudio();
        Time.timeScale = 1;
        SceneManager.LoadScene("Start");

    }
    public void OnResumeButtonClick()
    {
        //add by jason
        adManager.ShowFullScreenVideoAd();
        adManager.LoadFullScreenVideoAd();
        PlayUiAudio();
        BGMSource.Play();
        pauseUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void OnPauseButtonClick()
    {
        PlayUiAudio();
        BGMSource.Pause();
        pauseUI.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnRetryButtonClick()
    {
        //add by jason
        adManager.ShowFullScreenVideoAd();
        adManager.LoadFullScreenVideoAd();
        IniFavoriteFood();
        PlayUiAudio();
        BGMSource.Stop();
        Time.timeScale = 1;
        BGMSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        

    }
    public void OnFavoriteButtonClick()
    {
        //add by jason
        //adManager.ShowFullScreenVideoAd();
        //adManager.LoadFullScreenVideoAd();
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



    //****************************后台******************************

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
            fname = (i + 22).ToString();
            FavoriteDeFoods[i] = FavoriteUI.Find("Scroll View/Viewport/Content/bg/defoodUI/" + fname).GetComponent<FavoriteFoodUI>();
        }
    }
    //收藏初始化
    private void IniFavoriteFood()
    {
        for (int i = 0; i < 30; i++)
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

            FavoriteFood[i] = PlayerPrefs.GetInt("Food10" + fname);
        }
    }
    //收藏判断
    private void ChangeFavoriteFood(int id)
    {
        int n = id - 1001;
        if (FavoriteFood[n] == 1)
        {
            return;
        }
        else
        {
            FavoriteFood[n] = 1;
            PlayerPrefs.SetInt("Food"+id,FavoriteFood[n]);
            PlayerPrefs.SetInt("Food"+id+"New",1);
        }
    }


    //烹饪选中食物
    private void CookTouchFood()
    {
        if (HotPot.CheckPosNull(FoodTargetLocation))
        {
            //调整后台
            Conveyor.DeleteFood(TouchFood);
            HotPot.AddInHotPot(TouchFood, FoodTargetLocation);
            //锁定位置
            Vector3 pos = HotPot.GetCookPos(FoodTargetLocation);
            TouchFood.transform.position = pos;
            //开始烹饪
            TouchFood.GetComponent<Food>().OnCook();
        }
    }
    //返回烹饪状态
    private void ReturnCook()
    {
        Vector3 pos = HotPot.GetCookPos(TouchFood.GetComponent<Food>().NowPos);
        TouchFood.transform.position = pos;
        TouchFood.GetComponent<Food>().OnCook();
    }
    //吃菜
    public void EatDishFood()
    {
        IsEating = true;
        Food food = Dish.GetFood();
        food.CalculateValue();
        CheckCombo(food);
        ChangeEatPower(food.EatPowerValue,true);
        if (food.IsFire)
        {
            FireShowAdd();
        }
        AddScore(food.ScoreValue);
        ChangeFavoriteFood(food.GetFoodId());
        EatTime = 0;
        ComboTime = 0;
        FoodNumber++;
        Invoke("EndEat", 0.15f);
    }

    private void EndEat()
    {
        Dish.EatFood();
        IsEating = false;
    }

    //放菜到垃圾桶里
    private void PutFoodInTrash(GameObject food)
    {
        ChangeFavoriteFood(food.GetComponent<Food>().GetFoodId());
        //后台调整
        if (TouchFood.GetComponent<Food>().GetCookTime() == 0)
        {
            Conveyor.DeleteFood(TouchFood);
        }
        else
        { 
            HotPot.DeleteFood(TouchFood.GetComponent<Food>().NowPos);
        }
        Destroy(TouchFood);
        PlayTrashAudio();
        TouchFood = null;
    }

    //放菜到盘子里
    private void PutFoodInDish(GameObject food)
    {
        //后台调整
        HotPot.DeleteFood(TouchFood.GetComponent<Food>().NowPos);
        Dish.AddInDish(TouchFood);

        //放入盘子
        TouchFood.GetComponent<Food>().OnDish();
    }

    //喝水
    private void DrinkWater()
    {
        FireShowUse();
        //加分
        AddScore((int)(6666*((Second/10+WaterNumber)*0.01+1) * Random.Range(0.9f, 1.1f)), true);
        WaterNumber++;
    }

    //加分
    private void AddScore(int score,bool isNotEat=false)
    {
        //int addScore = (int)(score * FirePowerMultiple * (ComboNumber * 0.01 + 1));
        int addScore = 0;
        if (isNotEat)
        {
            addScore = score;
        }
        else
        {
            addScore = (int)(score * (ComboNumber * 0.02 + 1) * (WaterNumber*0.05 + 1) * Random.Range(0.95f, 1.05f));
        }

        if (addScore>0)
        {
            ShowAddScore(addScore);
            Score += addScore;
        }
    }

    //改变食力值
    private void ChangeEatPower(float value,bool isEat = false)
    {
        //限时模式不减食力值
        if (gameMode == GameMode.Time)
        {
            EatPower = 1000;
            return;
        }
        float ep;
        //连击奖励
        if (isEat && ComboNumber>3)
        {
            ep = EatPower + value*(1+(ComboNumber - 3)*0.1f);
        }
        else
        {
            ep = EatPower + value;
        }
        if (ep <= 250)
        {
            Fire.SetActive(true);
            Debug.Log("fire.setactive(true)");
            ReduceFoodDate();
        }
        else
        {
            Fire.SetActive(false);
        }
        if (ep > 1000)
        {
            ep = 1000;
        }
        else if (ep <= 0)
        {
            GameOver();
        }
        EatPower = ep;       
    }
    //改变火力值
   /* private void ChangeFirePower(int value,bool isEat = false)
    {
        int fp = FirePower + value;
        if (isEat)
        {
            fp += ComboNumber;
        }
        if (fp > 10000)
        {
            fp = 1000;
        }
        else if (fp < 0)
        {
            fp = 0;
        }
        FirePower = fp;
    }*/
    //判断连击
    private void CheckCombo(Food food)
    {
        if (food.IsPerfect())
        {
            ComboNumber += 1;
            ComboTime = 0;
            HighCombo = ComboNumber > HighCombo ? ComboNumber: HighCombo;
            if (ComboNumber>=3)
            {
                ShowCombo();
            }
        }
        else
        {
            InterruptCombo();
        }
    }
    //打断连击
    public void InterruptCombo()
    {
        EndCombo();
        ComboTime = 0;
        ComboNumber = 0;
    }
    //游戏结束
    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Gameover()");
        if (TouchFood!=null) { Destroy(TouchFood); }
        RecoveryFoodDate();
        Fire.SetActive(false);
        GameOverUI();
        PlayEndAudio();
        Time.timeScale = 0;
    }
    //按秒数执行的程序更新
    private void UpdateSecond()
    {
        if (GameTime > Second)
        {
            Second += 1;
            //ChangeEatPower(-EatPowerPadValue - FirePowerHurt);
            ChangeEatPower(-EatPowerPadValue);
            //ChangeFirePower(-FirePowerPadValue);
            if (EatTime * CookSpeedBase > 8)
            {
                //ChangeEatPower(-EatPowerPadValue - FirePowerHurt);
                ChangeEatPower(-EatPowerPadValue);
            }

            if (Second % 10 == 0)
            {
                switch (gameMode)
                {
                    case GameMode.Standard: CookSpeedBase += 0.025f;
                        break;
                    case GameMode.Time: CookSpeedBase += 0.075f;
                        break;
                }
                if (ServingSpeed > 1)
                {
                    ServingSpeed -= 0.25f;
                }
            }
            if (Second % 30 == 0)
            {
                if (EatPowerPadValue < 30)
                {
                    if (EatPowerPadValue < 15)
                    {
                        EatPowerPadValue += 1;
                    }
                    else
                    {
                        EatPowerPadValue += 0.5f;
                    }
                }
  
            }
        }
    }
    private void UpDataValue()
    {

        /*if (FirePower > 9000)
        {
            FirePowerPadValue = (int)(100 + (FirePower - 9000f) / 10);
            FirePowerHurt = (FirePower - 9000f) / 20 + 10;
            FirePowerMultiple = 4;
        }
        else if (FirePower > 5000)
        {
            FirePowerPadValue = (int)(20 + (FirePower - 5000f) / 50);
            FirePowerHurt = (FirePower - 5000f) / 400;
            FirePowerMultiple = 4;
        }
        else if (FirePower > 2000)
        {
            FirePowerPadValue = (int)(50 - (FirePower - 2000f) / 100);
            FirePowerHurt = 0;
            FirePowerMultiple = (FirePower - 2000f) / 1000 + 1;
        }
        else
        {
            FirePowerPadValue = 50;
            FirePowerHurt = 0;
            FirePowerMultiple = 1;
        }*/

        CookSpeed = CookSpeedBase + ComboNumber * 0.02f;

        if (ComboTime * CookSpeedBase > 6)
        {
            InterruptCombo();
        }
        if (ComboTime * CookSpeedBase > 5)
        {
            EndCombo();
        }
    }


    //计时
    private void UpDateTime()
    {
        if (isGameOver)
        {
            return;
        }
        GameTime += Time.deltaTime;
        EatTime += Time.deltaTime;
        ComboTime += Time.deltaTime;
        int s;
        int m;
        
        switch (gameMode)
        {
            case GameMode.Standard:
                s = (int)(GameTime % 60);
                m = (int)(GameTime / 60);
                break;
            case GameMode.Time:
                float tmt = (float)TimeModeTime - GameTime;
                if (tmt <= 0)
                {
                    GameOver();
                }
                s = (int)(tmt % 60);
                m = (int)(tmt / 60);
                break;
            default:
                s = (int)(GameTime % 60);
                m = (int)(GameTime / 60);
                break;
        }

        string ss;
        string ms;
        if (s < 10)
        {
            ss = "0" + s.ToString();
        }
        else
        {
            ss = s.ToString();
        }
        if (m < 10)
        {
            ms = "0" + m.ToString();
        }
        else
        {
            ms = m.ToString();
        }
        TimeText.text = ms + ":" + ss;
    }

    //辣椒状态显示和改变
    //动画改变
    private void FireShowChange()
    {
        FireShowAnimator.SetInteger("Fire", FireShowNumber);
        PandaAnimator.SetInteger("Fire", FireShowNumber);
    }
    //上升
    private void FireShowAdd()
    {
        if (FireShowNumber < 8)
        {
            FireShowNumber++;
            FireShowChange();
        }
        
        if (FireShowNumber == 8)
        {
            CupAnimator.SetBool("isLight", true);
        }
    }
    //归零 返回TRUE成功，否则失败
    private bool FireShowUse()
    {
        if (FireShowNumber == 8)
        {
            FireShowNumber = 0;
            CupAnimator.SetBool("isLight", false);
            FireShowChange();
            return true;
        }
        else
        {
            return false;
        }
    }
    //-------------------------------显示-----------------------------------



    /// <summary>
    /// 获取图片类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    string GetNumTypeString(E_NumType type)
    {
        string numType = "";
        switch (type)
        {
            case E_NumType.Type_1:
                numType = "type1_";
                break;
            case E_NumType.Type_2:
                numType = "type2_";
                break;
            default:
                numType = "type1_";
                break;
        }

        return numType;
    }

    /// <summary>
    /// 设置美术字
    /// </summary>
    /// <param name="num"></param>数值大小
    /// <param name="img"></param>图片数组
    /// <param name="type"></param>图片类型
    void SetPicNum(int num, Image[] img, E_NumType type = E_NumType.Type_1)
    {
        string str = num.ToString();//这里也可以数字设置表现:如num.ToString("N");
        if (str.Length > img.Length)
        {
            Debug.Log("长度超出");
            return;
        }
        int index = 0;
        for (int i = 0; i < img.Length; ++i)
        {
            if (i < str.Length)
            {
                img[i].gameObject.SetActive(true);

                //获取图片(这里是直接从Resources加载的)
                string path = "Sprite/" + GetNumTypeString(type) + str.Substring(i, 1);
                Sprite getSp = Resources.Load<Sprite>(path);
                img[index].sprite = getSp;
                img[index].SetNativeSize();
                index++;
            }
            else
            {
                img[i].gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 获得数字图片组
    /// </summary>
    /// <param name="tran"></param>根对象
    /// <returns></returns>
    private Image[] GetNumImages(Transform tran,int number = 9)
    {
        Image[] images = new Image[number];
        for (int i = 0; i < number; i++)
        {
            Transform t = tran.Find("N" + (i + 1).ToString());
            if (t == null) break;
            images[i] = t.GetComponent<Image>();
        }

        return images;
    }


    //显示加分UI
    private void ShowAddScore(int score)
    {
        if (IsAddScoreShow)
        {
            IniAddScore();
        }

        SetPicNum(score, AddScoreImageList);
        ScoreShowTime = Time.time;
        addScore.gameObject.SetActive(true);
        IsAddScoreShow = true;
        Invoke("IniAddScore",2f);
    }

    //初始化加分UI
    private void IniAddScore()
    {
        addScore.gameObject.SetActive(false);
        addScore.position = startPos;
        addScore.localScale = new Vector3(0.4f, 0.4f, 1);
        foreach (Image image in AddScoreImageList)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 255);
        }
        IsAddScoreShow = false;
    }
    //分数动画
    private void UpdateScoreShow()
    {
        if (IsAddScoreShow)
        {
            float l = Mathf.Lerp(0.4f, 1f, (Time.time - ScoreShowTime)*5f);
            addScore.position = Vector3.Lerp(addScore.position, endPos,(Time.time - ScoreShowTime)/6f);
            addScore.localScale = new Vector3(l, l, 1);
            float c = Mathf.Lerp(5f, 0, (Time.time - ScoreShowTime) / 2f);
            foreach (Image image in AddScoreImageList)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, c);
            }
        }
    }
    //显示连击UI
    private void ShowCombo()
    {
        SetPicNum(ComboNumber, ComboImageList,E_NumType.Type_2);
        ComboNumberView.GetComponent<Animator>().SetBool("IsNewCombo", true);
        ComboNumberView.GetComponent<Animator>().SetBool("IsStop", false);

        Invoke("ShowComboEnd",0.3f);
    }
    private void ShowComboEnd()
    {
        ComboNumberView.GetComponent<Animator>().SetBool("IsCombo", true);
        ComboNumberView.GetComponent<Animator>().SetBool("IsNewCombo", false);
    }
    //终止显示连击UI
    private void EndCombo()
    {
        ComboNumberView.GetComponent<Animator>().SetBool("IsNewCombo", false);
        ComboNumberView.GetComponent<Animator>().SetBool("IsStop", true);
    }
    private void GameOverUI()
    {
        EndScoreImageList = GetNumImages(endScore);
        SetPicNum(Score, EndScoreImageList);

        if (Score > HighScore)
        {
            HighScore = Score;
            switch (gameMode)
            {
                case GameMode.Standard: 
                    PlayerPrefs.SetInt("HighScore", HighScore);
                    if(Score>300000)
                    {
                        PlayerPrefs.SetInt("Food1018", 1);
                        PlayerPrefs.SetInt("Food1018New", 1);
                        PlayerPrefs.SetInt("Food1017", 1);
                        PlayerPrefs.SetInt("Food1017New", 1);
                        PlayerPrefs.SetInt("Food1016", 1);
                        PlayerPrefs.SetInt("Food1016New", 1);
                    }
                    else if(Score>200000)
                    {
                        PlayerPrefs.SetInt("Food1017", 1);
                        PlayerPrefs.SetInt("Food1017New", 1);
                        PlayerPrefs.SetInt("Food1016", 1);
                        PlayerPrefs.SetInt("Food1016New", 1);
                    }
                    else if(Score>100000)
                    {
                        PlayerPrefs.SetInt("Food1016", 1);
                        PlayerPrefs.SetInt("Food1016New", 1);
                    }
                    break;
                case GameMode.Time: 
                    PlayerPrefs.SetInt("TimeHighScore", HighScore);
                    break;
            }
        }
        switch (gameMode)
        {
            case GameMode.Standard:
                EndTimeText.text = TimeText.text;
                break;
            case GameMode.Time:
                EndTimeText.text = "03:00";
                break;
        }

        HighComboText.text = HighCombo.ToString();
        HighScoreText.text = HighScore.ToString();
        FoodNumberText.text = FoodNumber.ToString();
        EndUI.gameObject.SetActive(true);
    }
    //食力值更新
    private void UpdateEatPower()
    {
        EatPowerSlider.value = EatPower;
    }
    //分数更新
    private void UpDateScore()
    {
        TempScore = Mathf.Lerp(TempScore, (float)Score, Time.time);
        ScoreText.text = TempScore.ToString();
    }

    public static bool IsInTransform(Vector3 vector,Transform t)
    {
        float xMin=0,xMax=0,yMin=0,yMax=0;
        Vector3[] corners = new Vector3[4];
        t.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
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

    //===================属性和初始化======================
    /// <summary>
    /// 初始化数据
    /// </summary>
    private void IniGameData()
    {
        isGameOver = false;
        Score = 0;//分数
        EatPower = 1000;//食力值
        EatPowerPadValue = 15;//食力衰减值
        //FirePowerHurt = 0;//火力对食力的伤害

        /*FirePower = 0;//火力值
        FirePowerPadValue = 50;//火力衰减值
        FirePowerMultiple = 1;//火力对分数加成倍数*/

        ComboNumber = 0;//连击数

        CookSpeedBase = 1;//烹饪速度基数
        CookSpeed = 1;//烹饪速度倍数

        EatTime = 0;//为吃间隔时间
        ComboTime = 0;//为连击间隔时间

        GameTime = 0;
        Second = 0;

        HighCombo = 0;
        FoodNumber = 0;
        WaterNumber = 0;

        FireShowNumber = 0;

        gameMode = MenuManager.GameMode;
        switch (gameMode)
        {
            case GameMode.Standard:
                HighScore = PlayerPrefs.GetInt("HighScore", 0);
                ServingSpeed = 1.5f;
                break;
            case GameMode.Time:
                HighScore = PlayerPrefs.GetInt("TimeHighScore", 0);
                ServingSpeed = 1;
                break;
        }
        
        IsEating = false;
        IsDrinking = false;
    }

    private void IniGameSet()
    {
        _gameManager = this;
        //add by zhujian
        Fire = GameObject.Find("/Canvas_Game/Background/HotPot/Fire");
        //Debug.Log(Fire.name);
        AddScoreImageList = GetNumImages(addScore);
        ComboImageList = GetNumImages(ComboNumberView.Find("ComboNum").transform,5);
        startPos = addScore.position;
        endPos = ScoreEndPos.position;
        IniAddScore();
        EndTimeText = EndUI.transform.Find("Background/UI/GameDataUI/EndTime").GetComponent<Text>();
        HighComboText = EndUI.transform.Find("Background/UI/GameDataUI/HighCombo").GetComponent<Text>();
        HighScoreText = EndUI.transform.Find("Background/UI/GameDataUI/HighScore").GetComponent<Text>();
        FoodNumberText = EndUI.transform.Find("Background/UI/GameDataUI/FoodNumber").GetComponent<Text>();
        GetFavoriteFoods();
        if (gameMode == GameMode.Time)
        {
            LimitedTimeSupply.SetActive(true);
            EatPowerUI.SetActive(false);
        }
    }

    public static FoodData GetFoodDataIndex(int index)
    {
        int i = 0;
        foreach (FoodData foodData in GameFoodData)
        {
            i++;
            if (i == index)
            {
                return foodData;
            }
        }

        return null;
    }

    //add by zhujian
    private void ReduceFoodDate()
    {
        for(int i =0;i<GameFoodData.Count;i++)
        {
            GameFoodData[i].ChangeData();
        }
    }
    private void RecoveryFoodDate()
    {
        for (int i = 0; i < GameFoodData.Count; i++)
        {
            GameFoodData[i].RecoveryData();
        }
    }

    /// <summary>
    /// 设置本局食材
    /// </summary>
    /// <param name="foodData"></param>
    /// <returns></returns>
    public static bool SetGameFoodData(FoodData foodData)
    {
        if (foodData == null)
        {
            return false;
        }
        FoodData data = new FoodData(foodData);
        GameFoodData.Add(data);
        return true;
    }

    public static void CheckIniFoodData()
    {
        if (GameFoodData != null && GameFoodData.Count == 24)
        {
            isIniFoodData = true;
        }
    }
    public static float GetCookSpeed() => CookSpeed;
    public static float GetServingSpeed() => ServingSpeed;
    public static float GetGameTime() => GameTime;

    //声音播放

    private void PlayUiAudio()
    {
        UISource.Play();
    }
    private void PlayEatAudio()
    {
        EatSource.Play();
    }
    private void PlayEndAudio()
    {
        EndSource.Play();
    }
    private void PlayDrink1Audio()
    {
        DrinkSource1.Play();
    }
    private void PlayDrink2Audio()
    {
        DrinkSource2.Play();
    }
    private void PlayTrashAudio()
    {
        TrashSource.Play();
    }


}
