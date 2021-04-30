using UnityEngine;
using System.Collections;

public class RotateAroundAndLookAt : MonoBehaviour {

	public GameObject rotateCenter;		//旋转中心对象
	public float rotateSpeed = 10.0f;	//旋转速度

	//每帧执行一次：物体公转
	void Update () {
		if (rotateCenter) {		//当旋转中心对象设置时才进行物体公转
			transform.RotateAround (	
				rotateCenter.transform.position,	//旋转中心点
				rotateCenter.transform.up, 			//旋转轴：此处设置为为旋转中心的向上方向（正Y轴）
				Time.deltaTime * rotateSpeed		//旋转的角度，rotateSpeed表示旋转的速度，Time.deltaTime表示该帧执行的时间
			);
			transform.LookAt(rotateCenter.transform);	//使游戏对象始终朝向旋转中心
		}
	}
}
