using UnityEngine;
using System.Collections;

public class DominoCollide : MonoBehaviour {
	
	//当有物体与该物体即将发生碰撞时，调用OnCollisionEnter()函数
	void OnCollisionEnter(Collision collision)	
	{
		if (collision.gameObject.tag.Equals("Domino"))	//根据碰撞物体的标签来判断该物体是否为多米诺骨牌
			GetComponent<AudioSource>().Play();			//获取多米诺骨牌撞击音效的AudioSource组件并播放
	}

}
