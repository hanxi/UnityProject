using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public struct Hole {
        public bool isAppear;
        public int holeX;
        public int holeY;
        public GameObject mole;
    }

    public Hole[] holes;
    public float intervalPosX = 2, intervalPosY = 1;
    public GameObject holeObj;
    public GameObject moleObj;
    // Start is called before the first frame update
    void Start()
    {
        InitMap();
        InvokeRepeating("MoleAppear", 3f, 1f);
    }

    private void InitMap()
    {
        Vector2 originPos = new Vector2(-2,-2);
        holes = new Hole[9];

        for(int i=0; i<3; i++) {
            for (int j=0; j<3; j++) {
                int n = i*3+j;
                holes[n] = new Hole();
                holes[n].holeX = (int)(originPos.x + j*intervalPosX);
                holes[n].holeY = (int)(originPos.y + i*intervalPosY);
                holes[n].isAppear = false;
                Instantiate(holeObj, new Vector3(holes[n].holeX, holes[n].holeY, 0), Quaternion.identity);
                Debug.Log("hole:"+n+",x:"+holes[n].holeX+",y:"+holes[n].holeY);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CleanHoleState();
    }

    private void MoleAppear() {
        int id = UnityEngine.Random.Range(0, 9);
        while (holes[id].isAppear == true) {
            id = UnityEngine.Random.Range(0, 9);
        }
        Debug.Log("MoleAppear, id:"+id);
        holes[id].mole = Instantiate(moleObj, new Vector3(holes[id].holeX, holes[id].holeY, 0), Quaternion.identity);
        holes[id].isAppear = true;
    }

    private void CleanHoleState() {
        for (int i=0; i<9; i++) {
            if (holes[i].mole == null) {
                holes[i].isAppear = false;
            }
        }
    }
}
