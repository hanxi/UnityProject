using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverEffects : MonoBehaviour
{
	public Image darkImage;			//游戏失败时的黑屏效果
	public GameObject enemyPrefab;	//敌人预制件对象

    private Transform cameraTransform;		//摄像机的位置
    private Vector3 direction;				//摄像机偏移向量
    private GameObject player;				//玩家对象
    private GameObject[] enemies;			//游戏结束时场景中的敌人数组
	private GameObject[] gameoverEnemies;	//游戏失败时新生成的敌人数组

    private bool initialized;			//标记：游戏结束时是否初始化过
	private bool gameover;				//标记：游戏失败时是否初始化过
	private bool rankingPanelActive;	//标记：排行榜面板是否激活

	public GameObject rankingPanel;		//排行版面板

	//初始化函数，设置初始值
	void Start(){
		initialized = false;
		gameover = false;
		rankingPanelActive = false;
	}

	//游戏结束时场景效果的初始化
    void Init()
    {
		initialized = true;		//已初始化
		player = GameObject.FindGameObjectWithTag("Player");	//寻找场景中的游戏主角
		enemies = GameObject.FindGameObjectsWithTag("Enemy");	//寻找场景中的所有敌人对象

		GameObject.Destroy (GameObject.Find ("Gun"));	//获取枪械对象并删除
		foreach (GameObject enemy in enemies)			//删除场景中所有的游戏对象
			GameObject.Destroy (enemy);
       	direction = new Vector3(3.0f, 3.0f, 5.0f);		//设置摄像机偏移量

		Camera playerCamera = player.GetComponentInChildren<Camera> ();	//获取游戏主角的摄像机组件
		Camera gameOverCamera = GameObject.Find ("GameOverCamera").GetComponent<Camera> ();	//获取场景中游戏结束时的摄像机组件
		cameraTransform = gameOverCamera.transform;	

		cameraTransform.position = playerCamera.transform.position;			//将主角摄像机的position属性赋值给游戏结束摄像机
		cameraTransform.eulerAngles = playerCamera.transform.eulerAngles;	//将主角摄像机的rotation属性赋值给游戏结束摄像机

		playerCamera.enabled = false;	//禁用主角摄像机
		gameOverCamera.enabled = true;	//启用游戏结束摄像机
    }

	//摄像机切换行为
	void CameraBehavior(bool win)
    {
		if (!win) {	//若游戏失败
			rankingPanel.SetActive (rankingPanelActive);
			Invoke ("enablePanel", 3);		//三秒后显示排行版面板
			darkImage.color = Color.Lerp (	//屏幕瞬间黑屏，并逐渐变亮（线性插值）
				darkImage.color, 
				Color.clear, 
				0.2f * Time.deltaTime
			);
		}
		//游戏结束摄像机从当前位置逐渐平移至游戏主角的位置加上摄像机偏移量的位置（线性插值）
		cameraTransform.position = Vector3.Lerp (
			cameraTransform.position, 
			player.transform.position + direction, 
			0.01f
		);
		//游戏结束摄像机始终朝向游戏主角的位置
        cameraTransform.LookAt(player.transform);
    }

	void enablePanel(){
		rankingPanelActive = true;	//将排行榜面板激活标记设置为true
	}

	//游戏失败时的场景效果
	void GameOver(){
		gameover = true;	//已初始化
		darkImage.color = Color.black;	//将游戏失败的黑屏Image设置为黑色
		//计算新生成敌人的中心位置
		Vector3 enemyCenter = new Vector3 (
			player.transform.position.x - direction.x,
			player.transform.position.y,
			player.transform.position.z - direction.z);
		//计算敌人之间的方向向量
		Vector3 enemyVector = new Vector3 (direction.z, 0, -direction.x);
		enemyVector.Normalize ();

		gameoverEnemies = new GameObject[7];
		//循环生成敌人角色
		for (int i = -3; i <= 3; i++) {
			GameObject _enemy = (GameObject)GameObject.Instantiate (
				enemyPrefab,
				enemyCenter + i * enemyVector * 1.5f,
				Quaternion.identity);
			_enemy.transform.LookAt (player.transform.position + direction);			//使敌人面向敌人摄像机
			_enemy.transform.eulerAngles = new Vector3 (0, _enemy.transform.eulerAngles.y, 0);	//设置敌人的Rotation属性，确保其只在y轴旋转
			gameoverEnemies [i + 3] = _enemy;
		}

		//设置敌人对象刚体组件的约束，使敌人只在Y轴上受力作用
		foreach (GameObject enemy in gameoverEnemies) {
			enemy.GetComponent<Rigidbody> ().constraints =
				RigidbodyConstraints.FreezePositionX | 
				RigidbodyConstraints.FreezePositionZ | 
				RigidbodyConstraints.FreezeRotation;
		}
	}

	//每帧调用一次，根据游戏状态处理场景效果的变化
    void Update()
    {
        switch (GameManager.gm.gameState)
        {
        case GameManager.GameState.Playing:	//游戏进行时不进行效果处理
            return;
		case GameManager.GameState.Winning:	//游戏胜利
			if (!initialized) Init ();		//场景初始化
			CameraBehavior (true);			//摄像机行为
            break;
		case GameManager.GameState.GameOver://游戏失败
			if (!initialized) Init ();		//场景初始化
			CameraBehavior (false);			//摄像机行为
			if (!gameover) GameOver ();		//游戏失败效果
            break;
        }
    }
}
