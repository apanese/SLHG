using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FavoriteFoodUI : MonoBehaviour
{

    private int isGet;
    private int isNew;

    private void Awake()
    {
        ReadFood();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        ReadFood();
        ChangeNew();
        ChangeImage();
    }

    public void Close()
    {
        CloseNew();
    }

    private void ReadFood()
    {
        isGet = PlayerPrefs.GetInt("Food10" + name);
        isNew = PlayerPrefs.GetInt("Food10" + name + "New");
    }

    private void ChangeNew()
    {
        if (isNew == 1)
        {
            Transform t = transform.Find("new");
            t.gameObject.SetActive(true);
        }
    }

    private void CloseNew()
    {
        if (isNew == 1)
        {
            Transform t = transform.Find("new");
            t.gameObject.SetActive(false);
            isNew = 0;
            PlayerPrefs.SetInt("Food10" + name + "New",isNew);
        }
    }
    private void ChangeImage()
    {
        if (isGet == 1)
        {
            if (name.Equals("1016") || name.Equals("1017") || name.Equals("1018"))
            {
                switch (name)
                {
                    case "1016":
                        if (PlayerPrefs.GetInt("HighScore") > 100000)
                        {
                            Image icon = transform.GetComponent<Image>();
                            Sprite sp = Resources.Load("FavoriteUI/10" + name, typeof(Sprite)) as Sprite;
                            icon.sprite = sp;
                        }
                        break;
                    case "1017":
                        if (PlayerPrefs.GetInt("HighScore") > 200000)
                        {
                            Image icon = transform.GetComponent<Image>();
                            Sprite sp = Resources.Load("FavoriteUI/10" + name, typeof(Sprite)) as Sprite;
                            icon.sprite = sp;
                        }
                        break;
                    case "1018":
                        if (PlayerPrefs.GetInt("HighScore") > 300000)
                        {
                            Image icon = transform.GetComponent<Image>();
                            Sprite sp = Resources.Load("FavoriteUI/10" + name, typeof(Sprite)) as Sprite;
                            icon.sprite = sp;
                        }
                        break;
                }               
            }
            else
            {
                Image icon = transform.GetComponent<Image>();
                Sprite sp = Resources.Load("FavoriteUI/10" + name, typeof(Sprite)) as Sprite;
                icon.sprite = sp;
            }
        }
    }
}
