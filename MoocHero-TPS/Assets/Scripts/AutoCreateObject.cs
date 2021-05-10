using UnityEngine;
using System.Collections;

public class AutoCreateObject : MonoBehaviour {

	public GameObject createGameObject;			//自动生成的游戏对象

	public float minSecond=5.0f;				//随机生成游戏对象的最小时间
	public float maxSecond=10.0f;				//随机生成游戏对象的最大时间

	public GameObject targetTrace;				//生成游戏对象的追踪目标设置

	private float timer;		//生成时间间隔，记录从上次生成游戏对象到现在经过的时间
	private float createTime;	//生成时间，下次以生成游戏对象的时间，该值在[minSeconds,maxSeconds]随机生成

	//初始化，参数初始化
	void Start () {
		if(targetTrace==null)	//若追踪目标未设置，则自动将场景中的玩家设为追踪目标
			targetTrace=GameObject.FindGameObjectWithTag("Player");	
		timer = 0.0f;			//将生成时间间隔清零
		createTime = Random.Range (minSecond, maxSecond);	//在[minSeconds,maxSeconds]区间随机设置生成时间
	}

	//每帧执行，用于在随机时间内自动生成游戏对象
	void Update () {
		//若游戏状态不是游戏进行（Playing），则不生成游戏对象
		if (GameManager.gm != null 
			&& GameManager.gm.gameState != GameManager.GameState.Playing)	
			return;
		timer += Time.deltaTime;	//更新生成时间间隔，增加上一帧所花费的时间
		if (timer >= createTime) {	//当生成时间间隔大于等于生成时间时
			CreateObject ();		//调用CreateObject生成游戏对象
			timer = 0.0f;			//将生成时间间隔清零
			createTime = Random.Range (minSecond, maxSecond);	//在[minSeconds,maxSeconds]区间随机设置生成时间
		}
	}

	//生成游戏对象函数
	void CreateObject(){	
		Vector3 deltaVector = new Vector3 (0.0f, 5.0f, 0.0f);	//生成位置偏差向量
		GameObject newGameObject = Instantiate (				//生成游戏对象
			createGameObject, 					//生成游戏对象的预制件
			transform.position-deltaVector, 	//生成游戏对象的位置，为该脚本所在游戏对象的位置减去生成位置偏差向量
			transform.rotation					//生成游戏对象的朝向
		) as GameObject;
		if (newGameObject.GetComponent<EnemyTrace> () != null)	//设置敌人的追踪目标
			newGameObject.GetComponent<EnemyTrace> ().target = targetTrace;
	}
}
