using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms.Impl;

public class Conveyor : MonoBehaviour
{
    private static List<GameObject> ConveyorFood = new List<GameObject>();
    private Transform[] Pos = new Transform[6];
    private Animator animator;

    private float SpecialFood;
    private float DeFood;

    private float speed;
    private float startTime;

    private int MoveTimes;
    private float ServingTime;
    private void Awake()
    {
        SpecialFood = 0;
        DeFood = 0;
        ConveyorFood = new List<GameObject>();
        animator = GetComponent<Animator>();
        speed = GameManager.GetServingSpeed()/2;
        MoveTimes = 0;
        ServingTime = 0;
        startTime = 0;
        Pos[0] = GameObject.Find("Canvas_Game/Background/Conveyor/P1").transform;
        Pos[1] = GameObject.Find("Canvas_Game/Background/Conveyor/P2").transform;
        Pos[2] = GameObject.Find("Canvas_Game/Background/Conveyor/P3").transform;
        Pos[3] = GameObject.Find("Canvas_Game/Background/Conveyor/P4").transform;
        Pos[4] = GameObject.Find("Canvas_Game/Background/Conveyor/P5").transform;
        Pos[5] = GameObject.Find("Canvas_Game/Background/Conveyor/StartPos").transform;
    }
    // Start is called before the first frame update
    void Start()
    {

        animator.SetBool("IsStop", false);
        MoveConveyor();
        Serving();
    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateFoodPos();
        UpdateTime();
        UpdateConveyorState();
    }

    private void UpdateTime()
    {
        ServingTime += Time.deltaTime;
        if (ServingTime >= speed && MoveTimes == 0)
        {
            MoveConveyor();
            MoveTimes += 1;

        }
        if (ServingTime >= GameManager.GetServingSpeed())
        {
            MoveConveyor();
            Serving();
            ServingTime = 0;
            MoveTimes = 0;
        }
    }
    private void MoveConveyor()
    {
        if (ConveyorFood.Count >= 5)
        {
            animator.SetBool("IsStop", true);
        }
        speed = GameManager.GetServingSpeed() / 2;
        foreach (GameObject food in ConveyorFood)
        {
            food.transform.GetComponent<Food>().NowPos--;
        }
        startTime = Time.time;
    }
    //上菜
    public void Serving()
    {

        if (ConveyorFood.Count<5)
        {
            
            MakeFood();
        }
        
    }

    private void UpdateFoodPos()
    {
        int n = 0;
        foreach (GameObject food in ConveyorFood)
        {
            if (n < food.transform.GetComponent<Food>().NowPos&&food.GetComponent<Food>().GetTouch() == false)
            {
                //Debug.Log("food.transform.GetComponent<Food>().GetId():" + food.transform.GetComponent<Food>().GetFoodId());
                float a = MoveFood(food.transform.GetComponent<Food>().NowPos);
                food.transform.position = new Vector3(a, Pos[0].position.y, Pos[0].position.z);
            }
            else if(food.GetComponent<Food>().GetTouch()==false)
            {
                food.transform.position = new Vector3(Pos[n].position.x, Pos[0].position.y, Pos[0].position.z);
            }
            n++;
        }
    }
    private float MoveFood(int n)
    {
        return Mathf.Lerp(Pos[n].position.x, Pos[n-1].position.x, (Time.time - startTime) / speed);
    }

    //出现菜
    private void MakeFood()
    {
        if (ConveyorFood.Count >= 6)
        {
            return;
        }
        //抽取一个菜
        int index;
        if (Percent(15f - DeFood))
        {
            if (Percent(16f))
            {
                index = 19;
            }
            else if (Percent(32f))
            {
                index = 20;
            }
            else if (Percent(48f))
            {
                index = 21;
            }
            else if (Percent(64f))
            {
                index = 22;
            }
            else if (Percent(80f))
            {
                index = 23;
            }
            else
            {
                index = 24;
            }
            DeFood += 2f;
        }
        else
        {
            DeFood = 0;
            if ((Percent(5f + SpecialFood * 1f))&&PlayerPrefs.GetInt("Food1017") ==1)
            {
                index = 17;
                SpecialFood = 0;
            }
            else if ((Percent(5f + SpecialFood * 1.5f))&& PlayerPrefs.GetInt("Food1018") == 1)
            {
                index = 18;
                SpecialFood = 0;
            }
            else if((Percent(5f + SpecialFood * 1.5f))&& PlayerPrefs.GetInt("Food1016") == 1)
            {
                index = 16;
                SpecialFood = 0;
            }
            else { index = Random.Range(1, 16); SpecialFood += 1; }
        }
        //创建物体
        GameObject foodObject = Resources.Load<GameObject>("Food/"+GameManager.GetFoodDataIndex(index).Id.ToString());
        GameObject food = Instantiate(foodObject, Pos[5].position, Pos[5].rotation, transform);
        //输入菜的数据
        food.transform.GetComponent<Food>().SetFood(GameManager.GetFoodDataIndex(index));
        //add by zhujian
        //food.GetComponent<Food>().SetFood(GameManager.GetFoodDataIndex(index));
        ConveyorFood.Add(food);
    }

    private bool Percent(float p)
    {
        return (Random.Range(0f, 100f) <= p);  
    }
    public void UpdateConveyorState()
    {
        if (ConveyorFood.Count<5)
        {
            animator.SetBool("IsStop", false);
        }
        animator.speed = 3/speed;
    }

    public static void DeleteFood(GameObject food)
    {
        ConveyorFood.Remove(food);
    }

}
