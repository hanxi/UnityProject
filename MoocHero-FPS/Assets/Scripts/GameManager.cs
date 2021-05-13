using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	static public GameManager gm;				//静态游戏管理器，场景中唯一的游戏管理器实例

	public int TargetScore = 5;			//目标分数
	public enum GameState 				//游戏状态枚举
	{Playing,GameOver,Winning};
	public GameState gameState;			//游戏状态
	public GameObject player;			//游戏主角

	public GameObject playingCanvas;	//游戏进行时Canvas
	public Text scoreText;				//Text组件，用于显示当前游戏得分的文本信息
	public Text timeText;				//Text组件，用于显示当前游戏时间的文本信息
	public Slider healthSlider;			//Slider组件，用于显示玩家当前生命值血条
	public Image hurtImage;				//Image组件，用于玩家受伤时的屏幕变红效果

	public AudioClip gameWinAudio;				//游戏胜利音效
	public AudioClip gameOverAudio;				//游戏失败音效
	public GameObject gameResultCanvas;			//游戏结果Canvas
	public GameObject mobileControlRigCanvas;	//移动端控制UI

	public GameObject firstUserText;	//排名第一的玩家信息
	public GameObject secondUserText;	//排名第二的玩家信息
	public GameObject thirdUserText;	//排名第三的玩家信息
	public GameObject userText;			//本次玩家信息
	public Text gameMessage;			//游戏结果信息，提示玩家是否进入前三名

	private int currentScore;			//当前得分
	private float startTime;			//场景加载的时刻
	private float currentTime;			//从场景加载到现在所花的时间
	private PlayerHealth playerHealth;	//玩家生命值组件

	private bool cursor;					//鼠标光标是否显示
	private AudioListener audioListener;	//摄像机的AudioListener组件
	private Color flashColor = new Color (1.0f, 0.0f, 0.0f, 0.3f);	//玩家受伤时，hurtImage的颜色
	private float flashSpeed = 2.0f;								//hurtImage颜色的渐变速度

	private UserData firstUserData;		//排名第一的玩家的相关数据
	private UserData secondUserData;	//排名第二的玩家的相关数据
	private UserData thirdUserData;		//排名第三的玩家的相关数据
	private UserData currentUserData;	//当前玩家的相关数据
	private UserData[] userDataArray = new UserData[4];

	private bool isGameOver=false;		//标识，保证游戏结束时的相关行为只执行一次

	//初始化函数
	void Start () {
		Cursor.visible = false;	//禁用鼠标光标
		if (gm == null)			//静态游戏管理器初始化
			gm = GetComponent<GameManager> ();
		if (player == null)		//获取场景中的游戏主角
			player = GameObject.FindGameObjectWithTag ("Player");
		//获取场景中的AudioListener组件
		audioListener = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioListener> ();

		gm.gameState = GameState.Playing;	//游戏状态设置为游戏进行中
		currentScore = 0;					//当前得分初始化为0
		startTime = Time.time;				//记录场景加载的时刻

		playerHealth = player.GetComponent<PlayerHealth> ();	//获取玩家生命值组件，并初始化玩家生命值与HealthSlider参数
		if (playerHealth) {
			healthSlider.maxValue = playerHealth.startHealth;
			healthSlider.minValue = 0;
			healthSlider.value = playerHealth.currentHealth;
		}

		playingCanvas.SetActive (true);		//启用游戏进行中Canvas
		gameResultCanvas.SetActive(false);	//禁用游戏结果Canvas

		if(PlayerPrefs.GetString("Username")=="")	//若玩家未输入姓名，则将其姓名改为无名英雄
			PlayerPrefs.SetString("Username","无名英雄");
		//从本地保存的数据中获取前三名信息
		if (PlayerPrefs.GetString ("FirstUser") != "") {
			firstUserData = new UserData (PlayerPrefs.GetString ("FirstUser"));
		} else
			firstUserData = new UserData ();
		if (PlayerPrefs.GetString ("SecondUser") != "") {
			secondUserData = new UserData (PlayerPrefs.GetString ("SecondUser"));
		} else
			secondUserData = new UserData ();
		if (PlayerPrefs.GetString ("ThirdUser") != "") {
			thirdUserData = new UserData (PlayerPrefs.GetString ("ThirdUser"));
		} else
			thirdUserData = new UserData ();
		//根据GameStart场景中的声音设置，控制本场景中AudioListener的启用与禁用
		audioListener.enabled = (PlayerPrefs.GetInt ("SoundOff") != 1);
	}

	void Update () {
		//更新hurtImage的颜色（线性插值）
		hurtImage.color = Color.Lerp (
			hurtImage.color, 
			Color.clear, 
			flashSpeed * Time.deltaTime
		);

		//根据游戏状态执行不同的操作
		switch (gameState) {	
		//游戏进行时
		case GameState.Playing:		
			if (Input.GetKeyDown (KeyCode.Escape))		//输入Esc键，控制鼠标光标的可见性
				Cursor.visible = !Cursor.visible;
			if (playerHealth.isAlive == false)			//若玩家死亡，游戏状态切换到游戏失败
				gm.gameState = GameState.GameOver;
			else if (currentScore >= TargetScore) {		//若当前得分大于等于目标分数，游戏状态切换到游戏胜利
				currentScore = TargetScore;
				gm.gameState = GameState.Winning;
			}
			//否则，当前游戏状态还是游戏进行时状态
			else {							
				scoreText.text = "灭 敌 战 绩 ： " + currentScore;	//显示当前游戏得分
				healthSlider.value = gm.playerHealth.currentHealth;	//根据玩家当前生命值显示玩家生命值
				currentTime = Time.time - startTime;				//根据当前时刻与场景加载时刻计算游戏场景运行的时间
				timeText.text = "战 斗 时 间 ： " + currentTime.ToString ("0.00");	//显示已用时间
				if (mobileControlRigCanvas != null)					//启用移动端控制Canvas
					mobileControlRigCanvas.SetActive (true);
			}
			break;
		//游戏胜利
		case GameState.Winning:
			if (!isGameOver) {
				AudioSource.PlayClipAtPoint (gameWinAudio, player.transform.position);	//播放游戏胜利音效
				Cursor.visible = true;					//将鼠标光标显示
				playingCanvas.SetActive (false);		//禁用游戏进行中Canvas
				gameResultCanvas.SetActive (true);		//启用游戏结果Canvas
				if (mobileControlRigCanvas != null)		//禁用移动端控制Canvas
					mobileControlRigCanvas.SetActive (false);
				isGameOver = true;
				EditGameOverCanvas();	//编辑游戏结束Canvas中的排行榜
			}
			break;
		case GameState.GameOver:
			if (!isGameOver) {
				AudioSource.PlayClipAtPoint (gameOverAudio, player.transform.position);	//播放游戏失败音效
				Cursor.visible = true;					//将鼠标光标显示
				playingCanvas.SetActive (false);		//禁用游戏进行中Canvas
				gameResultCanvas.SetActive (true);		//启用游戏结果Canvas
				if (mobileControlRigCanvas != null)		//禁用移动端控制Canvas
					mobileControlRigCanvas.SetActive (false);
				isGameOver = true;
				EditGameOverCanvas();	//编辑游戏结束Canvas中的排行榜
			}
			break;
		}
	}

	//编辑游戏结束Canvas中的排行版
	void EditGameOverCanvas(){
		//根据当前玩家的姓名、得分、所用时间生成新的用户数据
		currentUserData = new UserData (PlayerPrefs.GetString("Username") + " 0 " + currentScore.ToString() + " " + currentTime.ToString("0.00"));
		currentUserData.isUser = true;			//该标识表示该数据是否为当前玩家数据
		//将当前玩家以及第一至第三名玩家的信息保存在userDataArray数组里
		userDataArray [0] = currentUserData;	
		int arrayLength = 1;
		if (firstUserData.order != "0")
			userDataArray [arrayLength++] = firstUserData;
		if (secondUserData.order != "0")
			userDataArray [arrayLength++] = secondUserData;
		if (thirdUserData.order != "0")
			userDataArray [arrayLength++] = thirdUserData;

		//排序函数
		mySort (arrayLength);
		//排序完毕后重新设置用户的名词
		foreach (UserData i in userDataArray) {
			if (i.isUser == true) {
				currentUserData = i;
				break;
			}
		}
		//若玩家进入前三名，则显示相应的游戏信息
		switch (currentUserData.order) {
		case "1":
			gameMessage.text = "恭喜你荣登慕课英雄榜榜首！";
			break;
		case "2":
			gameMessage.text = "恭喜你荣登慕课英雄榜榜眼！";
			break;
		case "3":
			gameMessage.text = "恭喜你荣登慕课英雄榜探花！";
			break;
		default:
			gameMessage.text = "";
			break;
		}

		//将更新后的排名信息显示在排行榜上
		Text[] texts;
		if (arrayLength > 0) {
			PlayerPrefs.SetString ("FirstUser", userDataArray [0].DataToString ());
			texts = firstUserText.GetComponentsInChildren<Text> ();
			LeaderBoardChange(texts,userDataArray [0]);
			arrayLength--;
		}
		if (arrayLength > 0) {
			PlayerPrefs.SetString ("SecondUser", userDataArray [1].DataToString ());
			texts = secondUserText.GetComponentsInChildren<Text> ();
			LeaderBoardChange(texts,userDataArray [1]);
			arrayLength--;
		}
		if (arrayLength > 0) {
			PlayerPrefs.SetString ("ThirdUser", userDataArray [2].DataToString ());
			texts = thirdUserText.GetComponentsInChildren<Text> ();
			LeaderBoardChange(texts,userDataArray [2]);
			arrayLength--;
		}

		//如果玩家未进入前三名，则显示玩家信息，并将显示玩家信息的Text内容加粗
		if (currentUserData.order != "1" && currentUserData.order != "2" && currentUserData.order != "3") {
			texts = userText.GetComponentsInChildren<Text> ();
			LeaderBoardChange (texts, currentUserData);
		} else {
			userText.SetActive (false);	//若玩家进入前三名，则不显示玩家信息，直接在前三名显示当前玩家的成绩
		}

	}

	//排序函数
	void mySort(int arrayLength){
		UserData temp;
		for (int i = 0; i < arrayLength; i++) {
			for (int j = i+1; j < arrayLength; j++) {
				if (userDataArray [i] < userDataArray [j]) {
					temp = userDataArray [j];
					userDataArray [j] = userDataArray [i];
					userDataArray [i] = temp;
				}
			}
		}
		//排序后更新玩家排名
		for (int i = 0; i < arrayLength; i++)
			userDataArray [i].order = (i + 1).ToString();
	}

	//将玩家信息显示在对应的text中
	void LeaderBoardChange(Text[] texts,UserData data){
		texts [0].text = data.username;
		texts [1].text = data.score.ToString();
		texts [2].text = data.time.ToString();
		if (data.isUser) {
			texts [0].fontStyle = FontStyle.Bold;
			texts [1].fontStyle = FontStyle.Bold;
			texts [2].fontStyle = FontStyle.Bold;
		}
	}

	//玩家得分
	public void AddScore(int value){
		currentScore += value;
	}
	//玩家扣血
	public void PlayerTakeDamage(int value){
		if (playerHealth != null)
			playerHealth.TakeDamage(value);
		hurtImage.color = flashColor;
	}
	//玩家加血
	public void PlayerAddHealth(int value){
		if (playerHealth != null)
			playerHealth.AddHealth(value);
	}

	//重新加载游戏场景
	public void PlayAgain(){
		SceneManager.LoadScene("GamePlay");
	}
	//加载游戏开始场景
	public void BackToMain(){
		SceneManager.LoadScene("GameStart");
	}

}
