using UnityEngine;
using System.Collections;

public class EnemyTrace : MonoBehaviour {

	public GameObject target;		//敌人的追踪目标
	public float moveSpeed=8.0f;	//敌人的移动速度
	public float minDist=2.2f;		//追踪距离，当敌人与目标的距离小于等于该值时，敌人不再追踪目标

	private float dist;				
	private Animator animator;				//敌人的Animator组件
	private EnemyHealth enemyHealth;		//敌人的生命值脚本

	//初始化，获取敌人的组件
	void Start () {
		animator = GetComponent<Animator> ();		//获取敌人的Animator组件	
		enemyHealth = GetComponent<EnemyHealth> (); //获取敌人的生命值脚本
	}

	//每帧执行一次，用于敌人追踪目标
	void Update () {
		if (enemyHealth!=null && enemyHealth.health <= 0) return;	//当敌人死亡时，敌人无法追踪目标
		if (target == null) {					//当追踪目标未设置时，敌人无法追踪目标
			animator.SetBool ("isStop", true);	//设置动画参数，将布尔型参数isStop设为true：敌人未追踪目标，播放停驻动画
			return;
		}
		dist = Vector3.Distance (transform.position, target.transform.position);	//计算敌人与追踪目标之间的距离
		//当游戏状态为游戏进行中（Playing）时
		if (GameManager.gm==null || GameManager.gm.gameState == GameManager.GameState.Playing) {			
			if (dist > minDist) {	//当敌人与目标的距离大于追踪距离时
				transform.LookAt (target.transform);				//敌人面向追踪目标
				transform.eulerAngles=new Vector3(0.0f,transform.eulerAngles.y,0.0f);	//设置敌人的Rotation属性，确保敌人只在y轴旋转
				transform.position += 
					transform.forward * moveSpeed * Time.deltaTime;	//敌人以moveSpeed的速度向追踪目标靠近
			}
			animator.SetBool ("isStop", false);	//设置动画参数，将布尔型参数isStop设为false：敌人追踪目标，播放奔跑动画
		}
	}
}
