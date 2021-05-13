using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerAttack : MonoBehaviour {

	public int shootingDamage = 1;				//玩家射击伤害
	public float shootingRange = 50.0f;			//玩家射击距离
	public AudioClip shootingAudio;				//射击音效
	public GameObject shootingEffect;			//射击时的粒子效果对象
	public Transform shootingEffectTransform;	//播放粒子效果的Transfrom属性

	private LineRenderer gunLine;		//线渲染器：射击时的激光射线效果
	private bool isShooting;			//玩家是否正在射击
	private Camera myCamera;			//摄像机组件
	private Ray ray;					
	private RaycastHit hitInfo;
	private GameObject instantiation;

	private static float LINE_RENDERER_START=0.02f;	//射线初始宽度
	private static float LINE_RENDERER_END=0.05f;	//射线末端宽度

	//初始化函数，获取组件
	void Start () {
		gunLine = GetComponent<LineRenderer> ();		//获取线渲染器组件
		if (gunLine != null) gunLine.enabled = false;	//在游戏开始时禁用线渲染器组件
		myCamera = GetComponentInParent<Camera> ();		//获取父对象的摄像机组件
	}

	//每帧执行一次，在Update函数后调用，实现玩家射击行为
	void LateUpdate () {	
		isShooting=CrossPlatformInputManager.GetButtonDown("Fire1");	//获取玩家射击键的输入
		//若在游戏进行中（Playing）获取玩家射击输入，则调用射击函数
		if (isShooting && (GameManager.gm==null || GameManager.gm.gameState == GameManager.GameState.Playing)) {
			Shoot ();
		} else if (gunLine != null)	//若射击条件未满足，表示未进行射击，禁用线渲染器
			gunLine.enabled = false;
	}

	//射击函数
	void Shoot()
	{
		AudioSource.PlayClipAtPoint (shootingAudio, transform.position);	//播放射击音效
		if (shootingEffect != null) {										//实例化玩家射击效果的粒子系统对象
			instantiation = Instantiate (shootingEffect, 
				shootingEffectTransform.position, 
				shootingEffectTransform.rotation) as GameObject;
			instantiation.transform.parent = transform;
		}
		ray.origin = myCamera.transform.position;		//设置射线发射的原点：摄像机所在的位置
		ray.direction = myCamera.transform.forward;		//设置射线发射的方向：摄像机的正方向
		if (gunLine != null) {
			gunLine.enabled = true;							//进行射击时，启用线渲染器（激光射线效果）
			gunLine.SetPosition (0, transform.position);	//设置线渲染器（激光射线效果）第一个端点的位置：玩家枪械的枪口位置
		}
		//发射射线，射线有效长度为shootingRange，若射线击中任何游戏对象，则返回true，否则返回false
		if (Physics.Raycast (ray, out hitInfo, shootingRange)) {
			if (hitInfo.transform.gameObject.tag.Equals ("Enemy")) {	//当被击中的游戏对象标签为Enemy，表明射线击中敌人
				//获取敌人生命值组件
				EnemyHealth enemyHealth = hitInfo.transform.gameObject.GetComponent<EnemyHealth> ();
				if (enemyHealth != null) {
					//调用EnemyHealth脚本的TakeDamage（）函数，对敌人造成shootingDamage的伤害
					enemyHealth.TakeDamage (shootingDamage);
				}
				if(enemyHealth.health > 0){
					//若敌人受伤未死亡，敌人将会因受到攻击而被击退
					hitInfo.collider.gameObject.transform.position += transform.forward * 2;
				}
			}
			if (gunLine != null) {
				gunLine.SetPosition (1, hitInfo.point);	//当射线击中游戏对象时，设置线渲染器（激光射线效果）第二个端点的位置：击中对象的位置
				gunLine.SetWidth (LINE_RENDERER_START, 	//射线在射程内击中对象时，需要根据击中对象的位置动态调整线渲染器（激光射线效果）的宽度
					Mathf.Clamp ((hitInfo.point - ray.origin).magnitude / shootingRange, 
						LINE_RENDERER_START, LINE_RENDERER_END));
			}
		} else if (gunLine != null) {
			//当射线未击中游戏对象时，设置线渲染器（激光射线效果）第二个端点的位置：射线射出后的极限位置
			gunLine.SetPosition (1, ray.origin + ray.direction * shootingRange);
			//射线在射程内未击中对象，直接设置射线的初始与末尾宽度
			gunLine.SetWidth (LINE_RENDERER_START, LINE_RENDERER_END);
		}
	}
}
