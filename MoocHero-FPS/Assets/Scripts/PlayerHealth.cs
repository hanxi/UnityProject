using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public int startHealth = 10;	//玩家的初始生命值
	public int currentHealth;		//玩家当前生命值
	public bool isAlive = true;		//玩家是否存活

	//初始化函数，设置玩家当前血量
	void Start () {
		currentHealth = startHealth;
	}
	
	//每帧执行一次，检测玩家是否存活
	void Update () {
		if (currentHealth <= 0) {
			isAlive = false;
		}
	}

	//玩家扣血函数，在GameManager脚本中调用
	public void TakeDamage(int damage){
		currentHealth -= damage;
		if (currentHealth < 0)
			currentHealth = 0;
	}

	//玩家加血函数，在GameManager脚本中调用
	public void AddHealth(int value){
		currentHealth += value;
		if (currentHealth > startHealth)	//加血后当前生命值不能超过初始生命值
			currentHealth = startHealth;
	}
}
