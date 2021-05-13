using UnityEngine;
//using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FPSPlayerMove : MonoBehaviour {

	public float moveSpeed = 6.0f;		//角色移动速度
	public float rotateSpeed = 3.0f;	//角色转向速度
	public float jumpVelocity = 5f;		//角色起跳时的速度

	private float miniMouseRotateX = -75.0f;		//摄像机旋转角度的最小值
	private float maxiMouseRotateX = 75.0f;			//摄像机旋转角度的最大值
	private float mouseRotateX;						//当前摄像机在X轴的旋转角度
	private bool isGrounded;						//玩家是否在地面上
	private float groundedRaycastDistance;			//在判定玩家是否在地面上时，向地面发射射线的射线长度

	private Camera myCamera;					//角色的摄像机子对象
	private Rigidbody rigid;					//角色刚体组件
	private CapsuleCollider capsuleCollider;	//角色的胶囊体碰撞体

	//初始化，获取组件，并计算相关值
	void Start () {
		myCamera = GetComponentInChildren<Camera> ();		//获取摄像机组件
		mouseRotateX = myCamera.transform.eulerAngles.x;	//将当前摄像机在X轴的旋转角度赋值给mouseRotateX
		rigid = GetComponent<Rigidbody> ();					//获取刚体组件
		capsuleCollider = GetComponent<CapsuleCollider> ();	//获取胶囊体碰撞体
		groundedRaycastDistance = capsuleCollider.height / 2 + 0.01f;	//计算向地面发射射线的射线长度
	}

	//每个固定时间执行一次，用于物理模拟
	void FixedUpdate()
	{
		//从玩家的位置垂直向下发出长度为groundedRaycastDistance的射线，返回值表示玩家是否该射线是否碰撞到物体，该句代码用于检测玩家是否在地面上
		isGrounded = Physics.Raycast(transform.position, -Vector3.up, groundedRaycastDistance);
	}

	//跳跃函数，用于FixedUpdate()中调用
	void Jump(bool isGround)
	{
		//当玩家按下跳跃键，并且玩家在地面上时执行跳跃相关函数
		if(CrossPlatformInputManager.GetButtonDown("Jump") && isGround &&(GameManager.gm == null || GameManager.gm.gameState == GameManager.GameState.Playing) )
		{
			//给玩家刚体组件添加向上的作用力，以改变玩家的运动速度，改变值为jumpVelocity
			rigid.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
		}
	}

	//每帧执行一次，用于获取玩家输入并控制角色的行为
	void Update () {
		//当游戏状态为游戏进行中时
		if (GameManager.gm == null || GameManager.gm.gameState == GameManager.GameState.Playing) 
		{
			float h = CrossPlatformInputManager.GetAxisRaw ("Horizontal");	//获取玩家水平轴上的输入
			float v = CrossPlatformInputManager.GetAxisRaw ("Vertical");	//获取玩家垂直轴上的输入
			Move (h, v);	//根据玩家的输入控制角色移动

			float rv = CrossPlatformInputManager.GetAxisRaw ("Mouse X");	//获取玩家鼠标垂直轴上的移动
			float rh = CrossPlatformInputManager.GetAxisRaw ("Mouse Y");	//获取玩家鼠标水平轴上的移动
			Rotate (rh, rv);	//根据玩家的鼠标输入控制角色转向

			Jump (isGrounded);	//玩家的跳跃函数
		}
	}

	//角色移动函数
	void Move(float h,float v){
		//玩家以moveSpeed的速度进行平移
		transform.Translate ((Vector3.forward * v + Vector3.right * h) * moveSpeed * Time.deltaTime);
	}

	//角色转向函数
	void Rotate(float rh,float rv){
		transform.Rotate (0, rv * rotateSpeed, 0);	//鼠标水平轴上的移动控制角色左右转向
		mouseRotateX -= rh * rotateSpeed;			//计算当前摄像机的旋转角度
		mouseRotateX = Mathf.Clamp (mouseRotateX, miniMouseRotateX, maxiMouseRotateX);	//将旋转角度限制在miniMouseRotateX与MaxiMouseRotateY之间
		myCamera.transform.localEulerAngles = new Vector3 (mouseRotateX, 0.0f, 0.0f);	//设置摄像机的旋转角度
	}
}
