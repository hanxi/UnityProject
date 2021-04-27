using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public struct Hole {
        public bool isAppear;
        public int holeX;
        public int holeY;
        public GameObject mole;
    }

    public Hole[] holes;
    public float intervalPosX = 2, intervalPosY = 1;
    public GameObject holeObj;
    public GameObject moleObj;
    public Timer timer;
    public Text scoreText;
    public int score = 0;
    public float appearFrequency = 0.5f; // 地鼠出现频率
    public bool canIncreaseMole = true; // 控制时间小于 15 秒时，只修改一次频率

    // 清空循环定时器，并开启一个循环定时器
    private void MoleAppearFrequency() {
        CancelInvoke();
        InvokeRepeating("MoleAppear", 0f, appearFrequency);
    }
    // Start is called before the first frame update
    void Start()
    {
        InitMap();
        MoleAppearFrequency();
        //InvokeRepeating("MoleAppear", 0f, 0.5f);
        timer.CountDown(true);
    }

    private void InitMap()
    {
        Vector2 originPos = new Vector2(-2,-2);
        holes = new Hole[9];

        for(int i=0; i<3; i++) {
            for (int j=0; j<3; j++) {
                int n = i*3+j;
                holes[n] = new Hole();
                holes[n].holeX = (int)(originPos.x + j*intervalPosX);
                holes[n].holeY = (int)(originPos.y + i*intervalPosY);
                holes[n].isAppear = false;
                Instantiate(holeObj, new Vector3(holes[n].holeX, holes[n].holeY, 0), Quaternion.identity);
                Debug.Log("hole:"+n+",x:"+holes[n].holeX+",y:"+holes[n].holeY);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CleanHoleState();
        if (timer.time < 0) {
            GameOver();
        }
        if (timer.time < 15 && canIncreaseMole == true) {
            appearFrequency -= 0.3f;
            canIncreaseMole = false;
            MoleAppearFrequency();
        }
    }

    private void GameOver()
    {
        timer.CountDown(false);
        CancelInvoke(); // 把所有 InvokeRepeating 取消
    }

    private void MoleAppear() {
        int id = UnityEngine.Random.Range(0, 9);
        for (int i=0; i<9; i++) {
            if (holes[id].isAppear == true) {
                id = UnityEngine.Random.Range(0, 9);
            }
        }
        if (holes[id].isAppear == true) {
            // 没随机到位置就退出
            return;
        }
        //Debug.Log("MoleAppear, id:"+id);
        holes[id].mole = Instantiate(moleObj, new Vector3(holes[id].holeX, holes[id].holeY, 0), Quaternion.identity);
        holes[id].mole.GetComponent<Mole>().id = id; // 给地鼠分配id
        holes[id].isAppear = true;
    }

    private void CleanHoleState() {
        for (int i=0; i<9; i++) {
            if (holes[i].mole == null) {
                holes[i].isAppear = false;
            }
        }
    }
}
