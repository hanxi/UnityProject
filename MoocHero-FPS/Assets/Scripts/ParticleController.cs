using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour {

	private ParticleSystem ps;	//粒子系统组件

	void Start(){
		ps = GetComponent<ParticleSystem> ();	//获取对象的粒子系统组件
		ps.Play ();								//播放粒子系统
		Destroy (gameObject, ps.duration);		//粒子系统播放完毕后删除该对象
	}
}
