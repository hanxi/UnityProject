using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

    public int shootingDamage = 1;				//玩家射击伤害
    public float shootingRange=50.0f;			//玩家射击范围
    public AudioClip shootingAudio;				//玩家射击音效
    public float timeBetweenShooting = 1.0f;	//射击之间的最小时间间隔（玩家射击动画为1秒，为了使得动画正常播放，该值最好>=1.0f）

	private Animator animator;			//玩家的Animator组件，用于控制玩家动画的播放
    private LineRenderer gunLine;		//玩家的线渲染器组件，用于控制玩家开枪发出的激光射线效果

    private float timer;				//攻击时间间隔，记录玩家从上次射击到现在经过的时间
    private Ray ray;
    private RaycastHit hitInfo;

	//初始化，获取对象组件，以及初始化变量
	void Start () {
		animator = GetComponentInParent<Animator>();	//获取玩家的Animator组件
		gunLine = GetComponent<LineRenderer>();			//获取玩家的线渲染器组件
        timer = 0.0f;		//将攻击时间间隔清零
	}

	//每帧执行一次，用于玩家的射击行为
	void Update () {
		//当玩家按下攻击键J，并且攻击间隔大于射击之间的最小时间间隔，执行射击相关行为
        if (Input.GetKeyDown(KeyCode.J) && timer>timeBetweenShooting)
        {
            timer = 0.0f;							//射击后将攻击时间间隔清零
			animator.SetBool("isShooting", true);	//设置动画参数，将isShooting布尔型参数设置为true，播放玩家射击动画
			Invoke("shoot", 0.5f);					//0.5秒后调用shoot() 射击函数
        }
		//否则，表示射击条件未满足
        else
        {
            timer += Time.deltaTime;	//更新攻击间隔，增加上一帧所花费的时间
            gunLine.enabled = false;	//将线渲染器设为禁用
			animator.SetBool("isShooting", false);	//设置动画参数，将isShooting布尔型参数设置为false，停止播放玩家射击动画
        }
	}

	//射击函数
    void shoot()
	{
		AudioSource.PlayClipAtPoint(shootingAudio, transform.position);	//在枪口位置播放射击音效
		ray.origin = Camera.main.transform.position;	//设置射线发射的原点：摄像机所在的位置
        ray.direction = Camera.main.transform.forward;	//设置射线发射的方向：摄像机的正方向
        gunLine.SetPosition(0, transform.position);		//设置线渲染器（开枪后的激光射线）第一个端点的位置：玩家枪械的枪口位置（本游戏对象）
        //发射射线，射线有效长度为shootingRange，若射线击中任何游戏对象，则返回true，否则返回false
		if (Physics.Raycast(ray, out hitInfo, shootingRange))
        {
            if (hitInfo.collider.gameObject.tag == "Enemy")	//当被击中的游戏对象标签为Enemy，表明射线射中敌人
            {
				//获取该名敌人的EnemyHealth脚本组件
                EnemyHealth enemyHealth = hitInfo.collider.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
					//调用EnemyHealth脚本的TakeDamage()函数，对敌人造成shootingDamage的伤害
					enemyHealth.TakeDamage(shootingDamage);	
                }
				if(enemyHealth.health>0)	//若敌人受伤且未死亡，敌人将会因受到攻击而被击退
					hitInfo.collider.gameObject.transform.position += transform.forward * 2;
            }
			gunLine.SetPosition(1, hitInfo.point);	//当射线击中游戏对象时，将线渲染器（开枪后的激光射线）第二个端点设为射线击中游戏对象的点
        }
		//若射线未射中游戏对象，则将线渲染器（开枪后的激光射线）第二个端点设为射线射出后的极限位置
        else gunLine.SetPosition(1, ray.origin + ray.direction * shootingRange);
		gunLine.enabled = true;	//将线渲染器（开枪后的激光射线）启用，显示玩家开枪后的效果。
    }
}
