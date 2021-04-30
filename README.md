# Unity 入门

## 学习 C# 语言

<https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/>

## 学习 Unity 编辑器

<https://www.icourse163.org/course/ZJICM-1449934195>

学习张帆的《一刻钟学会：游戏开发基础》笔记

### 开发环境

- Unity 2019.4.25f1 (64-bit)
- VSCode

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

#### 6. 实现计分功能

我实现和老师实现的不太一样，我是把 `score` 和 `scoreText` 放到 `GameController` 里的。

```c#
public class GameController : MonoBehaviour
{
    public Text scoreText; // 显示分数的 UI
    public int score = 0; // 分数
    // ...
}
```

然后 Mole.cs 里点击事件改为

```c#
    private void OnMouseDown() {
        Debug.Log("OnMouseDown");
        // beatenMole 销毁时 mole 会为 null
        gameController.holes[id].mole = Instantiate(beatenMole, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
        gameController.score++; // 分数加 1
        gameController.scoreText.text = "Score: " + gameController.score; // 显示分数
    }
```

#### 7. 游戏结束的功能

在 `GameController` 里的 `Update` 加入游戏结束的判断，如果时间没了就调用 `GameOver` 函数。

```c#
    void Update()
    {
        CleanHoleState();
        if (timer.time < 0) {
            GameOver();
        }
    }

    private void GameOver()
    {
        timer.CountDown(false); // 设置时间停止
        CancelInvoke(); // 把所有 InvokeRepeating 取消
    }
```

`Timer` 的 `CountDown` 修改如下，当时间停止时，修改时间和显示：

```c#
    // 设置是否开始倒计时
    public void CountDown(bool countDown) {
        canCountDown = countDown;
        if (canCountDown == false) {
            time = 0;
            timerText.text = "Game Over!!!";
        }
    }
```

#### 8. 修改地鼠出现频率

在 `GameController` 里新增两个变量，控制在 15s 改表地鼠出现频率。

```c#
    public float appearFrequency = 0.5f; // 地鼠出现频率
    public bool canIncreaseMole = true; // 控制时间小于 15 秒时，只修改一次频率

    // 清空循环定时器，并开启一个循环定时器
    private void MoleAppearFrequency() {
        CancelInvoke();
        InvokeRepeating("MoleAppear", 0f, appearFrequency);
    }
    void Start()
    {
        InitMap();
        MoleAppearFrequency();
        //InvokeRepeating("MoleAppear", 0f, 0.5f);
        timer.CountDown(true);
    }
```

新增函数 `MoleAppearFrequency` 并且在 `Start` 里面调用它。然后在 `Update` 里面也调用它。

```c#
    void Update()
    {
        CleanHoleState();
        if (timer.time < 0) {
            GameOver();
        }
        if (timer.time < 15 && canIncreaseMole == true) {
            appearFrequency -= 0.3f;
            canIncreaseMole = false; // 只能调用一次，所以需要这个标记
            MoleAppearFrequency();
        }
    }
```

同时优化下 `MoleAppear` ，不使用之前的死循环，只要判断 9 个位置都没空位了就提前退出。

```C#
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
```

#### 9. 实现锤子击打效果

隐藏鼠标，新建 `ChangeCursor.cs` 脚本

```c#
    public Sprite normalCursor; // 鼠标普通状态
    public Sprite hitCursor; // 鼠标点中状态
    public Image hamerImage; // 鼠标图片
    void Start()
    {
        Cursor.visible = false;
    }
```

添加锤子，新建 Image UI，命名为 `HammerImage` ，修改 Source Image 为 Hammer Sprite，并绑定 `ChangeCursor.cs` 脚本。绑定 Normal Cursor 为 Hammer Sprite，绑定 Hit Cursor 为 Hammer_Hit，绑定 Hammer Image 为 `HammerImage` 。

```c#
    void Update()
    {
        hamerImage.rectTransform.position = Input.mousePosition; // 设置图片的位置为鼠标位置
        if (Input.GetMouseButton(0)) {
            // 按下按钮则改图片为 hitCursor
            hamerImage.sprite  = hitCursor;
        } {
            hamerImage.sprite  = normalCursor;
        }
    }
```

这里应该是有 BUG， 在 2019.4 版本的 Unity 里点击鼠标没有替换图片。

#### 10. 实现音效

新增目录 `Audios` ，把两个音频文件拖进去。

然后给 Mole prefab 添加 Audio Source 组件，设置音频组件的 AudioClip 为 appear 音效。

再同样给 Mole_Beaten prefab 添加 Audio Source 组件，设置音频组件的 AudioClip 为 beaten 音效。

注意 Play On Awake 勾选。

#### 11. 重新开始游戏

实现一个按钮，按下 Restart 按钮重新开始游戏。使用老师提供的方法在 2019.4 版本的 Unity 里实现不了，点击按钮没反应。

使用这里的的第二种方法可以实现： <https://www.cnblogs.com/isayes/p/6370168.html>

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = this.GetComponent<Button> ();
        btn.onClick.AddListener (OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnClick()
    {
        SceneManager.LoadScene("WhackAmole");
    }
}
```

#### 12. 总结

按照课程一步一步跟着实战学习，熟悉了 Unity 编辑器的使用，熟悉了几个基础的 UI 控件。接下来不继续学张帆老师的课了，我看后面的游戏都是 2D 的小游戏，我想玩玩 3D 的游戏。所以挑选了另外一门课程，复旦大学姜忠鼎老师的课程《基于 Unity 引擎的游戏开发基础》

<https://www.coursera.org/learn/unity-yinqing-youxi-kaifa>

这门课程可以参加旁听的，是一个系列的课程，先学基础试试看，之后如果有兴趣的话再学习后面的课程。

## 基于 Unity 引擎的游戏开发基础

### 图形资源介绍

2D

- 位图 bitmap
  - 颜色深度，1bit 2 色，8bit 256 色，24 bit 16777216 色/65536 色+alpha 通道(16bit+8bit) ，32bit 16777216 色+alpha 通道
  - 常见位图格式：PSD,BMP，JPEG，PNG,DXTn
- 矢量图
  - 面向对象
  - 放大不会失真
  - 常见矢量图格式：AI,CDR,COL,SVG

3D

- 由三维软件构造
- 包括几何，材质，动画
- 文件格式
  - MB/MA(Autodesk Maya)
  - MAX(Autodesk 3DS Max)
  - BLENDER(Blender 免费)
  - FBX(Unity 默认)
  - OBJ

### 音频资源介绍

- 声波
- 声音的性质
- 声音的数字化（采样->编码）
- 文件格式
  - OGG
  - WAV
  - MP3
  - AIF
- 软件
  - Adobe Audition
  - Adobe Soundbooth
  - Audacity
  - 游戏引擎混音：Unity/Unreal

### Project 1：多米诺骨牌

#### 1. 基础知识

- 工程创建和资源导入
  - 新建项目，取名 Domino
  - Project -> 右键 -> Import New Asset
- 创建游戏对象
  - 哪些对象？
    - 摄像机
    - 光源
    - 立方体
  - 标签 Tag
  - 组件 Component
    - Transform 组件
    - 关系：相互独立，父子关系
    - 预制件：管理相同的游戏对象

#### 2. 摆放球和牌

金字塔资源是已经创建好的预制体，首先根据教程创建好三个小球和一个大球，小球摆放在金字塔顶，大球放在塔顶上方。

然后创建一个 Cube 用来做多米诺牌，把 Cube 做成预制，然后拷贝多个牌。主要是练习 Unity 编辑器的使用。

#### 3. 物理系统

PhysX NVIDIA

- 物理系统：重力，摩擦力，碰撞等
- 组件
  - 刚体（Rigidbody)
  - 恒定力（Constant Force)
  - 碰撞体（Collider)
- 管理器（Physics Manager)

这一节内容演示了如何添加刚体，以及如何添加物理材质。

#### 4. 三维物理渲染

- 网格（Mesh) 三角形网格
  - 正方体：12 个三角形
  - 球体：多个三角形
  - 网格过滤器（Mesh Filter)存放游戏对象的网格信息
  - 网格渲染器（Mesh Renderer)可以使用多个材质，和网格过滤器成对使用
- 材质（Material)
  - 创建：右键 -> Create -> Material
  - Materials Element
  - 选择着色器（Shader)
- 着色器（Shader）
  - 可编辑性
  - 不受到显卡渲染管线限制
  - 标准着色器（Standard Shader)
    - Rendering Mode
    - 预设值 Opaque 不透明
    - Transparent 制作头盔，玻璃等透明效果
    - Fade 制作淡入淡出效果
    - Cutout 允许透明和不允许透明同时存在并显示明显的边界（草地）
    - Albedo 表示光的反照率，描述物理的基本颜色
    - Normal Map 法向贴图，可以通过改变光的反射角度，使物理显得凹凸不平。
    - Emission 自发光
  - 天空盒（Skybox)
    - Skybox/6 Sided 绘制天空背景

这一节内容演示了如何渲染材质，已经添加了隐形的挡板，让球按着斜坡滚动。

#### 5. 光源 (Light)

- Type: 光源类型
  - Directional 方向光： 模拟极远处的光照，如地球上的太阳光
  - Point 点源光: 模拟光源向四周发出均匀的光线，如灯泡
  - Spot 聚光灯：向某个方向发出圆锥体光线，如手电筒
  - Area 面光源：无法模拟实时光照，用于光照烘培
- Color: 光源颜色
- Intensity: 光照强度
- Bounce Intensity: 反射光强度
- 环境光： Lighting -> Scene 设置环境光
  - Skybox ： 场景中天空盒设置
  - Ambient Source: 环境光来源
    - Skybox 天空盒
    - Gradient 梯度光
    - Color 单色光
  - Ambient Intensity : 环境光强度

演示了如何设置光源和天空盒。和演示视频不同的地方是，打开 Lightning 设置界面在 2019 里被移动到 Window -> Rendering 里面了。

#### 6. 摄像机 （Camera)

- 摄像机对象
- 摄像机组件
  - Clear Flags: 摄像机清除标记
    - Skybox(预设) 天空盒
    - Solid Color: 以某种颜色（纯色背景）
    - Depth Only: 以深度值较低的摄像机渲染的图案（用于多个摄像机的同时绘制）
    - Don't Clear: 不清除（不清楚影子）
  - Projection： 摄像机投射方式
    - Perspective 透视，远小近大
    - Orthographic 正交，大小不变
  - Field of View : 摄像机视角
  - Clipping Planes: 摄像机远近剪切平面
  - Depth: 摄像机深度
  - GUI Layer: 显示 GUI 控件
  - Flare Layer： 耀斑特效
  - Audio Listener： 音频监听

演示了添加新的摄像机，并绑定脚本控制摄像机围绕顶端旋转。然后添加摄像机组件，控制按 S 按键切换摄像机。

#### 7. 音频（Audio)

- AudioListerner 音频监听
- AudioSource 音频源
  - AudioClip 音频片段
  - Mute 是否静音
  - Play On Awake 是否自动播放
  - Loop 是否循环
  - Volume 音量大小
  - Pitch 音调高低
- AudioManager 音频系统管理器 Edit -> Poject Setting -> Audio
  - Global Volume 全局音量大小设置
  - Disable Unity Audio 是否停用 Unity 的音频系统

演示了添加撞击音效。

#### 8. 项目部署

- 项目构建 Build

这个就不详细介绍了，之前张帆的课程里玩了一遍。

### Unity 编程语言 — C#编程

### Unity 脚本编程 — Project 2：慕课英雄 MOOC HERO（第三人称射击简易版）

### Unity 高级特性与移动平台开发 — Project 3：慕课英雄 MOOC HERO（第一人称射击完整版）
