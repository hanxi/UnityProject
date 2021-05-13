using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	public int health=2;	//敌人的生命值
	public int value=1;		//玩家击杀敌人后所获得的分数
	public AudioClip enemyHurtAudio;		//敌人的受伤音效
	public GameObject hurtEffect;			//敌人受伤效果的粒子系统对象
	public Transform hurtEffectTransform;	//粒子系统对象实例化时的Transform属性

	private Animator anim;			//敌人的Animator组件
	private Collider colli;			//敌人的Collider组件
	private Rigidbody rigid;		//敌人的rigidbody组件

	private GameObject instantiation;

	//初始化，获取敌人的组件
	void Start(){
		anim = GetComponent<Animator> ();	//获取敌人的Animator组件
		colli = GetComponent<Collider> ();	//获取敌人的Collider组件
		rigid = GetComponent<Rigidbody> ();	//获取敌人的Rigidbody组件
	}

	//敌人受伤函数，用于PlayerAttack脚本中调用
	public void TakeDamage(int damage){
		if (hurtEffect != null) { 				//实例化敌人受伤效果的粒子系统对象
			instantiation = Instantiate (hurtEffect, 
				hurtEffectTransform.position, 
				hurtEffectTransform.rotation) as GameObject;
			instantiation.transform.parent = transform;
		}
		health -= damage;						//敌人受伤扣血
		if (enemyHurtAudio != null)				//在敌人位置处播放敌人受伤音效
			AudioSource.PlayClipAtPoint (enemyHurtAudio, transform.position);
		if (health <= 0) {						//当敌人生命值小于等于0时，表明敌人已死亡
			if (GameManager.gm != null) {	
				GameManager.gm.AddScore (value);//玩家获得击杀敌人后得分
			}
			anim.applyRootMotion = true;	//设置Animator组件的ApplyRootMotion属性，使敌人的移动与位移受动画的影响
			anim.SetTrigger ("isDead");		//设置动画参数，设置isDead的Trigger参数，播放敌人死亡动画
			colli.enabled = false;			//禁用敌人的collider组件，使其不会与其他物体发生碰撞
			rigid.useGravity = false;		//因为敌人的collider组件被禁用，敌人会因重力穿过地形系统下落，取消敌人受到的重力可以避免该现象
			Destroy (gameObject, 3.0f);			//3秒后删除敌人对象
		}
	}
}
