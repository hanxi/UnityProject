using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText; // UI 控件用来显示倒计时
    public float time = 30.0f; // 初始化倒计时
    private bool canCountDown = false; // 标记是否开始倒计时
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canCountDown == true) {
            // 扣倒计时
            time = time - Time.deltaTime;
            Debug.Log("Update time:"+time);
            // 修改倒计时显示
            timerText.text = "Time: " + time.ToString("f1");
        }
    }

    // 设置是否开始倒计时
    public void CountDown(bool countDown) {
        this.canCountDown = countDown;
    }
}
