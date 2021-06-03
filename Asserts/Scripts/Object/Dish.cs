using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour
{
    private static GameObject DishFood;
    private static Transform _Dish;
    public Transform DishPos;
    private static float yMax;
    private static float yMin;
    private static float xMin;
    private static float xMax;
    private void Awake()
    {
        _Dish = transform.GetComponent<Transform>();
        SetDishPos();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Food GetFood() => DishFood.GetComponent<Food>();
    public static void EatFood()
    {
        if (DishFood != null)
        {
            DishFood.GetComponent<Food>().Eat();
            DishFood = null;
        }
    }

    public static void AddInDish(GameObject food)
    {
        food.transform.parent = _Dish;
        DishFood = food;
        food.transform.position = _Dish.transform.position;
        food.GetComponent<Food>().InDish();
    }
    public static bool IsInDish(Vector3 vector)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(vector);
        if (pos.x > xMin && pos.x < xMax)
        {
            if (pos.y > yMin && pos.y < yMax)
            {
                return true;
            }
        }
        return false;
    }
    private void SetDishPos()
    {
        Vector3[] corners = new Vector3[4];
        DishPos.gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
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
    }

    public static bool CheckNull() => DishFood == null;

}
