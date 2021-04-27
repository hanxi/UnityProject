using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callback : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() {
        Debug.Log("Awake");
    }

    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
        gameObject.transform.position += new Vector3(0,0.01f,0);
    }

    private void LateUpdate() {
        Debug.Log("LateUpdate");
    }

    private void OnEnable() {
        Debug.Log("OnEnable");
    }

    private void OnDisable() {
        Debug.Log("OnDisable");
    }
}
