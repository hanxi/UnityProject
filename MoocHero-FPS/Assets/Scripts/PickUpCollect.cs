using UnityEngine;
using System.Collections;

public class PickUpCollect: MonoBehaviour {

	public enum PickUpType { score, health };	//收集物类型枚举：加分 / 加血
	public PickUpType pickUpType;				//该收集物的类型
	public int value = 2;						//该收集物的价值
	public AudioClip collectedAudio;			//收集物品时播放的音效

	//当进入收集物的收集范围（触发器）时调用
	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player") {	//进入的对象为玩家时
			if (GameManager.gm != null) 		
			{
				if (pickUpType == PickUpType.score)		//若收集物类型是加分，则进行玩家加分
					GameManager.gm.AddScore (value);
				else if(pickUpType==PickUpType.health)	//若收集物类型是加血，则进行玩家加血
					GameManager.gm.PlayerAddHealth (value);
			}
			if (collectedAudio!=null)	//收集物被收集，播放收集音效
				AudioSource.PlayClipAtPoint (collectedAudio, transform.position);
			Destroy(gameObject);		//销毁收集物对象
		}
	}
}
