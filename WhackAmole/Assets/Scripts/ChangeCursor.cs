using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCursor : MonoBehaviour
{
    public Sprite normalCursor;
    public Sprite hitCursor;
    public Image hamerImage;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        hamerImage.rectTransform.position = Input.mousePosition; // 设置图片的位置为鼠标位置
        if (Input.GetMouseButton(0)) {
            // 按下按钮则改图片为 hitCursor
            hamerImage.sprite  = hitCursor;
        } {
            hamerImage.sprite  = normalCursor;
        }
    }
}
