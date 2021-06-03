using UnityEngine;
using System.Collections;

public class ScreenAdapt : MonoBehaviour
{


    /*
     * 屏幕适配
    */

    /// <summary>
    /// 屏幕比例：宽度
    /// </summary>
    public int screenRatioX = 9;
    /// <summary>
    /// 屏幕比例：高度
    /// </summary>
    public int screenRatioY = 16;

    void Start()
    {

        //获取主摄像机
        Camera ca = transform.GetComponent<Camera>();

        //获取屏幕的宽度
        float xScreen = Screen.width;
        Debug.Log("xScreen:"+xScreen);

        //获取屏幕的高度
        float yScreen = Screen.height;
        Debug.Log("yScreen:" + yScreen);

        //设置的显示比例 与 设备的显示比例 差值
        float proportion = (screenRatioX / screenRatioY) - (xScreen / yScreen);

        //当设置的比例值大于设备比例值
        if (proportion > 0)
        {

            //计算新的屏幕高度 屏幕的宽度 / 屏幕的宽度比例 * 屏幕高度的比例
            float yNow = (xScreen / screenRatioX) * screenRatioY;

            //计算屏幕的高度变化比例 新的屏幕高度 / 初始的屏幕高度
            float modifys = yNow / yScreen;

            //设置主摄影机的 Viewport Rect, x = 0, y = (1 - 高度变化比例) / 2, w = 1, h = 高度变化比例
            ca.rect = new Rect(0.0f, (1 - modifys) / 2, 1.0f, modifys);
        }


        //当设置的比例值小于设备比例值
        if (proportion < 0)
        {
            //计算新的屏幕宽度 屏幕的高度 / 屏幕的高度比例 * 屏幕宽度的比例
            float xNow = (yScreen / screenRatioY) * screenRatioX;

            //计算屏幕的宽度变化比例 新的屏幕宽度 / 初始的屏幕宽度
            float modifys = xNow / xScreen;

            //设置主摄影机的 Viewport Rect, x = (1 - 宽度变化比例) / 2, y = 0 , w = 宽度变化比例, h = 1
            ca.rect = new Rect((1 - modifys) / 2, 0.0f, modifys, 1.0f);
        }


    }
}