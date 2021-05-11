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

这门课程只讲解基本 C# 编程

- C# 基础语法
  - 跟 C/C++ 一样的语法结构
  - 判断 if/else if/else , switch case
  - 循环 for/while/do while/ break/ continue
- C# 变量与函数
  - 基本数据类型
    - 整数
      - sbyte 8bit [-2^7,2^7-1]
      - short 16bit [-2^15,2^15-1]
      - int 32bit [-2^31,2^31-1]
      - long 64bit [-2^63,2^63-1]
    - 浮点数
      - float
      - double
      - decimal
    - bool
    - char
    - string
  - 字符串连接符号为 `+`
  - 枚举 enum
  - 结构 struct
  - 数组 `int[] arr = {1,2,3};`
  - 值类型和引用类型
    - 值类型：数值类型，布尔类型，结构类型
    - 引用类型：类，委托，数组，接口
    - `ref` 调用函数前需要初始化
    - `out` 需要在函数实现里初始化
- C# 面向对象概念
  - 语法类似 C++
  - 析构函数会被自动调用
  - 只能继承一个基类，可以继承多个接口
  - 抽象类 abstract， 使用 override 重写方法

我看视频里的例子都比较简单，这一节就没写代码例子了。

### Unity 脚本编程 — Project 2：慕课英雄 MOOC HERO（第三人称射击简易版）

#### 1. 脚本基础

- 鼠标右键 -> Create -> C# Script
- 从 Unity 里创建的 C# 脚本都会继承 MonoBehaviour 类。
- 使用 `Debug.Log` 打印日志。

#### 2. 脚本的生命周期

- 事件函数
  - Reset
  - Awake
  - OnEnable/OnDisable
  - Start
  - OnDestory
  - 物理循环 FixedUpdate, OnTriggerXXX, OnCollisionXXX
  - OnMouseXXX
  - 游戏逻辑循环 Update, LateUpdate
  - OnGUI
- 脚本之间的执行顺序：以堆栈的方式，先设置，后执行。即：最先榜单的脚本最后执行。
  - 可以使用 MonoManger 修改执行顺序的优先级

#### 3. 多米诺骨牌的脚本讲解

- 摄像机围绕金字塔旋转
  - 每帧执行一次：物体自转 `Domino/Assets/Scripts/SelfRotate.cs`
  - 每帧执行一次：物体公转 `Domino/Assets/Scripts/RotateAroundAndLookAt.cs`
- 多米诺骨牌的撞击声
  - 当有物体与该物体即将发生碰撞时，调用 OnCollisionEnter()函数 `Domino/Assets/Scripts/DominoCollide.cs`
- 大球向下冲击效果
  - 每隔固定时间执行一次 FixedUpdate ，用于物理模拟 `Domino/Assets/Scripts/ObjectAddForce.cs`
- 多米诺骨牌中的摄像机切换 `Domino/Assets/Scripts/CameraSwitch.cs`
  - 检测 S 键按下 `Input.GetKeyDown(KeyCode.S)`
  - 切换摄像机，修改摄像机对象的 `enabled` 属性

#### 4. 地形系统(Terrain)

- 创建地形： Hierarchy 视图中鼠标右键 -> Create -> 3D Object -> Terrain
- 地形系统组件
  - Transform
  - Terrain Collider 地形系统碰撞体
  - Terrain 7 种编辑工具
    - Raise/Lower Terrain(升高/降低地形)
      - Brushes 笔刷：用于绘制地形的笔刷样式
      - Brushes Size 笔刷尺寸：用于确定笔刷大小
      - Opacity 绘制强度：用于确定每次点击后地形升高/降低的强度
    - Paint Height(喷绘高度)用于将地形绘制到指定高度
    - Smooth Height(平滑高度)用于平滑地形高度
    - Paint Texture(绘制纹理) 用于绘制地形系统的地表纹理
      - Target Strength(目标强度)：纹理绘制的最大影响程度
    - Place Trees(种植树)
      - Add Tree
      - Edit Tree
      - Remove Tree
      - Number of Trees 随机种植树的个数
      - Keep Existing Trees 是否保留已种植的树
      - Tree Density: 树绘制的密度
      - Tree Height：绘制的树高度，可选择随机范围
      - Lock Width to Height: 确定树的宽度是否一致
      - Tree Width： 绘制的树宽度，可选择随机范围
      - Random Tree Rotate: 确定树的朝向是否随机
    - Paint Details(绘制细节)
      - Add Grass Texture
      - Add Detail Mesh 绘制草丛
    - Terrain Setting（地形设置）用于设置地形系统的相关参数
      - Base Terrain
        - Draw: 是否呈现地形系统
        - Pixel Error: 像素容差，表示显示地形网格时允许的像素误差
        - Base Map Dist: 用于设定高分辨率贴图的显示范围
        - Cast Shadows: 地形是否投射阴影
      - Tree & Detail Objects
        - Draw : 是否呈现树和细节
        - Detail Distance： 设定超过摄像机多少距离的细节将会停止渲染
        - Tree Distance： 表示树的显示距离，与摄像机距离超过该值的树将停止渲染
      - Resolution
        - Terrain Width: 地形系统的宽度
        - Terrain Length : 地形系统的长度
        - Terrain Height : 地形系统的高度

#### 5. 地形系统(Terrain) 演示

演示中需要用到 Unity 的 Standard Assets ，Unity 2019 中没有默认带这个资源了，需要手动下载。

下载地址： https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-for-unity-2018-4-32351

另外绘制地形的前面几个按钮合并到一个按钮了。有些许按钮位置不一样，但实际功能都是一样的。

Skybox 的设置前面也设置过，熟悉的话会可以比较快的使用。

#### 6. 动画片段与角色替身 (Animation Clip & Avatar)

- MeCanim 动画系统
- Mecanim Workflow(Mecanim 工作流)
  - 资源准备和导入 3Dmax Maya
  - 角色的建立
    - 人形角色: 动画重定向
    - 一般角色
  - 角色的运动
- 导入模型与动画资源
  - Project 视图鼠标右键 -> Import Package -> Characters
- 模型与动画设置
  - Models 模型网格
  - Rig 模型骨骼
  - Animations 模型动画片段
- 向场景中添加任务模型
- Animation Clip (动画片段)
  - Project 视图选择动画资源
  - Animations 中查看片段
- 分割 Animation Clip
  - 在 Inspector 视图中的 Clips 列表下选择需要分割的动画片段
  - 使用动画预览确认动画片段的分割点（帧数）
  - 设置动画片段的起始和末尾帧，并修改动画片段名称
  - 新建动画片段，重复上述步骤完成动画分割
- Animation Clip 首尾一致检查
  - Loop Time 首尾姿势是否一致
  - Root Transform Roation 检查首尾 Rotation 属性是否一致（直线运动才会一致）
  - Root Transform Position(Y) 首尾检查 Position 属性 Y 轴分量是否一致（行走奔跑才一致，跳跃的不一致）
  - Root Transform Position(XZ) 原地上跳的才会一致
- Avatar(角色替身)
  - 人形骨架
  - 动画重定向
  - 动画类型 Animaition Type
    - Humanoid (人形动画)
    - Generic
    - Legacy
    - None
  - Avatar 的配置

#### 7. 演示：导入角色模型和分割动画片段

分割动画就是观察动画的起始帧，然后设置帧数。

#### 8. 动画状态机（Animation State Machine)

- Animator 组件
  - 选择游戏对象 -> Compent 菜单 -> Miscellaneous -> Animator
  - Animator Controller 动画控制器
  - Avatar 角色替身
  - Apply Root Motion: 角色的位移或者旋转是否由动画片段控制。
- Animator Controller
  - 动画状态机 Animation State Machine
  - 动画层与身体遮罩 Animation Layer & Avatar Mask
- WASD 控制角色移动，空格控制跳跃
- Animation State Machine 用于角色动画片段的播放与切换
  - 状态： idel, walk, run
  - 状态过度
  - 参数
- 导入 Import Package -> Characters
- Ethan 模型
- Animator 视图
  - 创建和设置新状态
  - 状态的名称
  - 状态播放的动画片段 Motion
  - 动画片段的播放速度 Speed
- 添加动画参数
- 动画状态的过度： Make Transition
  - Has Exit Time 动画状态过度是否有退出时间
  - 动画状态的过度曲线
  - 状态过度条件设置 IsStop == True
  - 使用脚本控制动画参数

演示：给敌人模型创建控制器，并设置动画状态（停驻，奔跑，攻击，死亡），动画参数和动画状态的过度。

#### 9. 动画层与身体遮罩

- 奔跑时射击，跳跃时射击，行走时射击
- 下落动画，奔跑动画，合成
- Animation Layer 动画层
  - Weight
  - Mask: 身体遮罩
    - 启动的关节受动画的控制而产生动作
    - 禁用的关节不受动画的控制
    - Project 视图 -> 鼠标右键 -> Create -> Avatar Mask
    - 红色表示禁用
    - 绿色表示启用

演示： 给玩家模型创建动画控制器，并设置动画状态（停驻，奔跑，射击，跳跃），动画参数以及动画状态的过度。

#### 10. 玩家移动控制

- 玩家运动实现
  - 使用 W, S 键控制玩家前进，后退
  - 使用 A，D 键控制玩家左转，右转
  - 使用空格键控制玩家跳跃
  - 摄像机跟随玩家进行移动和旋转

演示： 把 RobotPlayer 预制拖到场景中，然后把摄像机拖到 RobotPlaer 下面。然后添加 PlayerMove.cs 脚本。

使用 `Input.GetAxisRaw` 函数获取玩家的输入，默认 A，D 为水平输入，W,S 为垂直输入。使用 `transform.Translate` 函数控制玩家前进和后退，使用 `transform.Rotate` 函数控制水平旋转。

```c#
//每帧执行一次，用于玩家的位移与旋转
void Update () {
    float h = Input.GetAxisRaw("Horizontal"); //获取玩家水平轴上的输入
    float v = Input.GetAxisRaw("Vertical"); //获取玩家垂直轴上的输入
    MoveAndRotate(h, v); //根据玩家在水平、垂直轴上的输入，调用玩家的位移与旋转函数
}
```

物理相关函数在 `FixedUpdate` 里调用，使用 `Physics.Raycast` 函数来判断玩家是否在地面上。

```c#
//每个固定时间执行一次，用于物理模拟
void FixedUpdate()
{
    //从玩家的位置垂直向下发出长度为groundedRaycastDistance的射线，返回值表示玩家是否该射线是否碰撞到物体，该句代码用于检测玩家是否在地面上
    isGrounded = Physics.Raycast(transform.position, -Vector3.up, groundedRaycastDistance);
    Jump(isGrounded); //调用跳跃函数
}
```

#### 11. 玩家生命值和射击

玩家生命值,  `PlayerHealth.cs`

```c#
public int health = 10; //玩家的生命值
public bool isAlive = true; //玩家是否存活
```

玩家攻击, `PlayerAttack`

```c#
//每帧执行一次，用于玩家的射击行为
void Update () {
    //当玩家按下攻击键J，并且攻击间隔大于射击之间的最小时间间隔，执行射击相关行为
    if (Input.GetKeyDown(KeyCode.J) && timer>timeBetweenShooting)
    {
        timer = 0.0f; //射击后将攻击时间间隔清零
        animator.SetBool("isShooting", true); //设置动画参数，将isShooting布尔型参数设置为true，播放玩家射击动画
        Invoke("shoot", 0.5f); //0.5秒后调用shoot() 射击函数
    }
    //否则，表示射击条件未满足
    else
    {
        timer += Time.deltaTime; //更新攻击间隔，增加上一帧所花费的时间
        gunLine.enabled = false; //将线渲染器设为禁用
        animator.SetBool("isShooting", false); //设置动画参数，将isShooting布尔型参数设置为false，停止播放玩家射击动画
    }
}
```

按 J 键攻击，同时限制攻击频率为 timeBetweenShooting 。射击有 0.5 秒动作，所以使用 `Invoke` 延迟 0.5 秒执行 shoot 函数。

演示： 给 RobotPlayer 添加 PlayHealth 脚本，给 RobotPlayer 的子对象 GunBarrelEnd 添加 PlayAttack 脚本，将 SightBeadUI(准星) 预制拖入场景。

#### 12. 敌人的追踪逻辑

EnemyTrace.cs

使用 `Vector3.Distance` 接口计算敌人与玩家的距离，如果距离大于设定的值，则追踪目标。

使用 `transform.LookAt` 接口设置敌人朝向玩家。

#### 13. 敌人的生命值，分数与攻击

EnemyHealth.cs

- 敌人被玩家攻击时，减少生命值
- 敌人受伤时，出现流血效果，并发出受伤的声音
  - 使用 `AudioSource.PlayClipAtPoint` 播放声音
- 敌人死亡则倒地消失，并给玩家加分数

EnemyAttack.cs

- 当玩家进入敌人攻击范围，敌人攻击玩家，玩家受到伤害
- 敌人攻击时抬手击打，并播放敌人攻击音效
- 敌人攻击有时间间隔

`OnTriggerStay` 和 `OnTriggerExit` 事件监测是否有物体处于攻击范围和离开攻击范围。

演示： 给敌人添加 Capsule Collider 组件，并勾选 is Trigger 选项，给敌人对象绑定 EnemyHealth 和 Enemy Attack 脚本。

#### 14. 游戏管理脚本的实现

GameManager.cs

- 管理游戏状态（游戏进行中/胜利/失败）
- 管理玩家积分
- 管理场景中对象之间的交互
- 显示游戏装（玩家生命值和玩家得分）

`Update` 中管理游戏状态：分别表示游戏进行（Playing）、游戏失败（GameOver）、游戏胜利（Winning）。

演示：创建空对象 GameManager ，添加 GameManager 脚本，将 GameMessageUI 预制件拖入场景，设置 GameManager 脚本的相关属性。

#### 15. 敌人自动生成

AutoCreateObject.cs

- 在给定的地点动态生成敌人，这里设定在金字塔顶端生成
- 每次生成敌人的时间间隔随机

```c#
createTime = Random.Range (minSecond, maxSecond);
```

使用 `Instantiate` 创建对象。

```c#
//生成游戏对象函数
void CreateObject() {
    Vector3 deltaVector = new Vector3 (0.0f, 5.0f, 0.0f); //生成位置偏差向量
    GameObject newGameObject = Instantiate ( //生成游戏对象
    createGameObject, //生成游戏对象的预制件
    transform.position-deltaVector, //生成游戏对象的位置，为该脚本所在游戏对象的位置减去生成位置偏差向量
    transform.rotation //生成游戏对象的朝向
  ) as GameObject;
  if (newGameObject.GetComponent<EnemyTrace> () != null) //设置敌人的追踪目标
    newGameObject.GetComponent<EnemyTrace> ().target = targetTrace;
}
```

### Unity 高级特性与移动平台开发 — Project 3：慕课英雄 MOOC HERO（第一人称射击完整版）
