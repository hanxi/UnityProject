using UnityEngine;
using System.Collections;

public class ObjectAddForce : MonoBehaviour {

	public int force;	//作用力大小

	//每隔固定时间执行一次，用于物理模拟
	void FixedUpdate () {
		gameObject.GetComponent<Rigidbody>()		//获取游戏对象上的刚体组件
			.AddForce (new Vector3(0,-force,0));	//给刚体添加方向向下的作用力
	}

}
