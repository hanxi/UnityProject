using UnityEngine;
using System.Collections;

public class SelfRotate : MonoBehaviour {

	public float rotateSpeed = 40.0f;	//旋转速度

	//每帧执行一次：物体自转
	void Update () {
		//物体以世界坐标系的向上方向（正Y轴）方向，以rotateSpeed的速度进行顺时针自转
		//Time.deltaTime表示该帧的执行时间，Time.deltaTime * rotateSpeed表示该帧总共自转的角度
		transform.Rotate (Vector3.up, Time.deltaTime * rotateSpeed);	
	}
}
