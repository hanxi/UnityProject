# Unity 入门

## 学习 C# 语言

<https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/>

## 学习 Unity 编辑器

<https://www.icourse163.org/course/ZJICM-1449934195>

学习张帆的《一刻钟学会：游戏开发基础》笔记

### 熟悉 Unity 的关键回调函数

1. 创建场景
2. 创建组件，拷贝组件（ctrl + D)
3. Unity 的关键回调函数

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callback : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() {
        Debug.Log("Awake");
    }

    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
        // 沿着Y轴，每一帧移动0.01米
        gameObject.transform.position += new Vector3(0,0.01f,0);
    }

    private void LateUpdate() {
        Debug.Log("LateUpdate");
    }

    private void OnEnable() {
        Debug.Log("OnEnable");
    }

    private void OnDisable() {
        Debug.Log("OnDisable");
    }
}
```

## 实战小游戏

### 1. 做一个迷宫小游戏

1. 使用 Cube 拼地图，按住 v 鼠标拖动会自动对齐
2. 下载资源 Mini first person controller ，并将 First person controller bare 拖进迷宫
3. 新增 `ExitGame.cs` 脚本，并实现按 ESC 按钮退出游戏。

```c#
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
```

### 2. 做一个打地鼠游戏

#### 1. 导入资源

1. 设置 Sort Layer ， 新增 Layer: Ground, Hole, Mole
2. 将 Hole, Mole 存为 prefab

#### 2. 生成洞口

`public GameObject holeObj;` 把脚本挂在 ground 上，holeObj 设为 hole 的 prefeb 。

`holes` 数组用来存放洞口，0~9 个洞口

```txt
6(-2,0)   7(0,0)   8(2,0)
3(-2,1)   4(0,-1)  5(2,-1)
0(-2,-2)  1(0,-2)  2(2,-2)
```

```txt
a[m][n] 映射到一维数组b[k]的公式：k=i*n+j，m,n 分别表示二维数组的行数和列数，i为元素所在行，j 为元素所在列。同时 0<=i<m, o<=j<n
```

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public struct Hole {
        public bool isAppear;
        public int holeX;
        public int holeY;
    }

    public Hole[] holes;
    public float intervalPosX = 2, intervalPosY = 1;
    public GameObject holeObj;
    // Start is called before the first frame update
    void Start()
    {
        InitMap();
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

    }
}
```

#### 3. 随机生成地鼠

使用 `InvokeRepeating` 定时调用 `MoleAppear` 函数。

```c#
    void Start()
    {
        InitMap();
        InvokeRepeating("MoleAppear", 2f, 1f);
    }
    private void MoleAppear() {
        int id = UnityEngine.Random.Range(0, 9);
        while (holes[id].isAppear == true) {
            id = UnityEngine.Random.Range(0, 9);
        }
        Debug.Log("MoleAppear, id:"+id);
        holes[id].mole = Instantiate(moleObj, new Vector3(holes[id].holeX, holes[id].holeY, 0), Quaternion.identity);
        holes[id].isAppear = true;
    }

    private void CleanHoleState() {
        for (int i=0; i<9; i++) {
            if (holes[i].mole == null) {
                holes[i].isAppear = false;
            }
        }
    }
```

地鼠调用 `Destroy` 延迟删除自己。

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
```

#### 4. 鼠标左键打击地鼠

用到函数 `OnMouseDown()` 来检测鼠标点击事件。

流程就是点击后，把打中状态的地鼠放到地鼠孔那里，然后删除普通地鼠。

打中状态的地鼠设为 1 秒后消失。把 `gameController.holes[id].mole` 赋值为打中状态的地鼠，当打中状态的地鼠删除时，它就会变成 null , 然后在 `CleanHoleState` 里面就会更新 isAppear 的状态。

```c#
    private void OnMouseDown() {
        Debug.Log("OnMouseDown");
        // beatenMole 销毁时 mole 会为 null
        gameController.holes[id].mole = Instantiate(beatenMole, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
```

#### 5. 实现游戏计时

需用用到 `Time.deltaTime` 来倒计时。新建一个 `Timer.cs` 。

```c#
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
```
