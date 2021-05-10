using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public int health = 10;			//玩家的生命值
	public bool isAlive = true;		//玩家是否存活

	//每帧执行一次，检测玩家是否存活
	void Update () {	
		if (health <= 0)
			isAlive = false;
	}

	//玩家扣血函数，用于GameManager脚本中调用
	public void TakeDamage(int damage){
		health -= damage;
		if (health < 0) 
			health = 0;
	}
}
