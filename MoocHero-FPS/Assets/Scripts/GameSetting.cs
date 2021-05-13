using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour {
	
	public GameObject InitSubPanel;		//开始界面的初始面板
	public GameObject StartSubPanel;	//点击“开始”按钮后的面板
	public GameObject OptionSubPanel;	//点击“选项”按钮后的面板

	public InputField usernameInputField;	//用户名输入框组件
	public Toggle soundToggle;				//声音开关

	//“开战”按钮调用的函数
	public void StartGame(){
		PlayerPrefs.SetString ("Username", usernameInputField.text);	//将用户输入的用户名保存在本地，标识名为“Username”
		SceneManager.LoadScene("GamePlay");								//加载游戏场景
	}

	//声音开关
	public void SwitchSound(){
		if (soundToggle.isOn) PlayerPrefs.SetInt ("SoundOff", 0);	//当声音开关开启时，将声音开关设置保存在本地，标识名为“SoundOff”，值为0
		else PlayerPrefs.SetInt ("SoundOff", 1);					//当声音开关开启时，将声音开关设置保存在本地，标识名为“SoundOff”，值为1
	}

	//“退出”按钮调用的函数
	public void ExitGame(){
		Application.Quit ();	//退出游戏
	}

	//初始化函数
	void Start () {
		ActiveInitPanel ();	//调用ActiveInitPanel函数，启用初始面板，禁用其他面板
	}

	//启用初始面板，禁用其他面板
	public void ActiveInitPanel(){
		InitSubPanel.SetActive (true);
		StartSubPanel.SetActive (false);
		OptionSubPanel.SetActive (false);
	}

	//启用开始面板，禁用其他面板
	public void ActiveStartPanel(){
		InitSubPanel.SetActive (false);
		StartSubPanel.SetActive (true);
		OptionSubPanel.SetActive (false);
	}

	//启用选项面板，禁用其他面板
	public void ActiveOptionPanel(){
		InitSubPanel.SetActive (false);
		StartSubPanel.SetActive (false);
		OptionSubPanel.SetActive (true);
	}
}
