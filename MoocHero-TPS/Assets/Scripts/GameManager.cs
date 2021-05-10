using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	static public GameManager gm;		//静态游戏管理器，场景中唯一的游戏管理器实例

	public GameObject player;			//玩家对象
	private PlayerHealth playerHealth;	//玩家的生命值脚本

	public int TargetScore=10;			//游戏获胜的目标分数
	private int currentScore;			//游戏当前得分

	//游戏状态枚举，分别表示游戏进行（Playing）、游戏失败（GameOver）、游戏胜利（Winning）
	public enum GameState {Playing,GameOver,Winning};	
	public GameState gameState;			//游戏状态变量

    public Text scoreText;				//GUI控件，用于显示当前游戏得分的文本信息
    public Text healthText;				//GUI控件，用于显示玩家当前的生命值

	//初始化，获取相关组件，并初始化变量
	void Start () {
		//初始化游戏管理器类
		gm = GetComponent<GameManager> ();	
		//根据标签获取玩家对象
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player");	

		//游戏开始前初始化变量
		currentScore = 0;
		//获取玩家对象的玩家生命值脚本
		playerHealth = player.GetComponent<PlayerHealth> ();
	}

	//每帧执行一次，用于游戏状态的检测与切换，以及处理当前游戏状态需要执行的语句
	void Update () {
		switch (gameState) {	//根据当前游戏状态来决定要执行的语句

		//当游戏状态为游戏进行中（Playing）状态时
		case GameState.Playing:	
            scoreText.text = "Score:" + currentScore;			//将显示当前游戏得分的文本信息更改为“Score:分数”
			healthText.text = "Health:" + playerHealth.health;	//将显示玩家当前生命值的文本信息更改为“Health:生命值”
			//若玩家死亡，游戏状态更改为游戏失败（GameOver）
			if (playerHealth.isAlive == false)
				gm.gameState = GameState.GameOver;
			//若当前得分大于目标分数，游戏状态更改为游戏胜利（Winning）
			else if (currentScore >= TargetScore)
				gm.gameState = GameState.Winning;
			break;
		
		//当游戏状态为游戏胜利（Winning）状态时
		case GameState.Winning:
            SceneManager.LoadScene("level1");	//加载场景level1
            break;
		
		//当游戏状态为游戏失败（GameOver）状态时
		case GameState.GameOver:
			SceneManager.LoadScene("level1");	//加载场景level1
            break;
		}
	}

	//玩家击杀得分
	public void AddScore(int value){
		currentScore += value;
	}

	//玩家受伤扣血
	public void PlayerTakeDamage(int value){
		if (playerHealth != null)
			playerHealth.TakeDamage(value);
	}
}
