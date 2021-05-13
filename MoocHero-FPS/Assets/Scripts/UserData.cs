using UnityEngine;
using System.Collections;

public class UserData{

	public string username;
	public string order;
	public int score;
	public float time;
	public bool isUser;

	public UserData(){
		username = "";
		order = "0";
		score = -1;
		time = -1;
		isUser = false;
	}

	public UserData(string _username,string _order,int _score,float _time,bool _isUser){
		username = _username;
		order = _order;
		score = _score;
		time = _time;
		isUser = _isUser;
	}

	public UserData(string s){
		char[] c = { ' ' };
		string[] p = s.Split (c, 5);
		username = p [0];
		order = p [1];
		score = int.Parse (p [2]);
		time = float.Parse (p [3]);
		isUser = false;
	}

	public string DataToString(){
		return username + " " + order.ToString () + " " + score.ToString () + " " + time.ToString ();
	}

	public static bool operator <(UserData data1,UserData data2){
		if (data1.score < data2.score)
			return true;
		else if (data1.score == data2.score && data1.time > data2.time)
			return true;
		else
			return false;
	}
	public static bool operator >(UserData data1,UserData data2){
		if (data1.score > data2.score)
			return true;
		else if (data1.score == data2.score && data1.time < data2.time)
			return true;
		else
			return false;
	}
}
