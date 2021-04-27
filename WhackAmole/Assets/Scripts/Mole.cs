using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : MonoBehaviour
{
    public GameObject beatenMole; // 用来点中时替换成这个对象
    public int id; // 给地鼠分配id

    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindObjectOfType<GameController>();
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown() {
        Debug.Log("OnMouseDown");
        // beatenMole 销毁时 mole 会为 null
        gameController.holes[id].mole = Instantiate(beatenMole, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
