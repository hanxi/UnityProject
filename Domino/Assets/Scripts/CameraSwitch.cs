using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {

	public Camera mainCamera;	//主摄像机
	public Camera orthCamera;	//正交摄像机

	//摄像机状态初始化
	void Start(){
		mainCamera.enabled = true;	//启用主摄像机
		orthCamera.enabled = false;	//禁用正交摄像机
	}

	//每帧调用一次：摄像机切换
	void Update () {
        if (Input.GetKeyDown(KeyCode.S)){	//当玩家按下键盘上的“S”键时
			mainCamera.enabled = !mainCamera.enabled;	//切换主摄像机的启用与禁用状态
			orthCamera.enabled = !orthCamera.enabled;	//切换正交摄像机的启用与禁用状态
        }
	}
}
