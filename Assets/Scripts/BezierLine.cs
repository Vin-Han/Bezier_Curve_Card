using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierLine : MonoBehaviour
{
    public ButtonClick beginBTN;         // 操作按钮
    public InputField countInput;        // 输入数量

    public RectTransform Sencor;    // 传感器，判定点击位置的东西
    public Image HeadImage;         // 头部图标
    public Image BaseImage;         // 身体图标

    private bool ifWorking = false; // 是否在工作
    private int pointCount = 10;    // 链条节点数量

    private Vector3 basePos;        // 原点坐标
    private Vector3 biasPos;        // 鼠标坐标（与原点统一坐标系时

    private BezierCurve bezierCurve = new BezierCurve();    // 贝塞尔曲线计算类
    private List<RectTransform> transList = new List<RectTransform>();      // 储存的物体引用
    private void Awake()
    {
        beginBTN.downEvent  = InitOBJList;
        beginBTN.upEvent    = DisposeOBJList;
    }

    void Update()
    {
        if (ifWorking)
        {
            DrawBezierPoint(Input.mousePosition);
        }
    }
    // 初始化所有的物件节点和必要数据
    private void InitOBJList()
    {
        if (countInput != null)
        {
            if(int.TryParse(countInput.text, out pointCount))pointCount += 3;
            else pointCount = 4;
        }

        if (pointCount < 2) return;
        biasPos = BaseImage.rectTransform.position - BaseImage.rectTransform.localPosition;
        basePos = BaseImage.rectTransform.localPosition;
        if (transList.Count < 2)
        {
            transList.Add(Sencor);
            transList.Add(HeadImage.rectTransform);
        }
        RectTransform BasePoint = (RectTransform)BaseImage.rectTransform.parent;
        for (int i = 0; i < pointCount - 2; i++)
        {
            GameObject tempOBJ = GameObject.Instantiate(BaseImage.gameObject, BasePoint);
            Image tempImage = tempOBJ.GetComponent<Image>();
            transList.Add(tempImage.rectTransform);
        }
        ifWorking = true;
    }
    // 销毁所有物体节点和位置复原
    private void DisposeOBJList()
    {
        Sencor.SetPositionAndRotation(basePos,Quaternion.Euler(0,0,0));
        HeadImage.rectTransform.SetPositionAndRotation(basePos, Quaternion.Euler(0, 0, 0));

        for (int i = transList.Count - 1; i > 1; i--)
        {
            Destroy(transList[i].gameObject);
            transList.RemoveAt(i);
        }
        ifWorking = false;
    }
    // 画所有的贝塞尔节点
    private void DrawBezierPoint(Vector3 mousePos)
    {
        Vector3 relativePos = mousePos - biasPos;
        List<Vector2> pointList = bezierCurve.GetBezierCurve(relativePos, basePos, transList.Count);
        transList[0].localPosition = pointList[0];      // 标志物不转向
        for (int i = 1; i < transList.Count; i++)
        {
            transList[i].localPosition = pointList[i];
            transList[i].up = (transList[i - 1].position - transList[i].position).normalized;
        }
    }





    /// 创建一条两点之间的线
    public void CreateLine(Vector2 start, Vector2 end, GameObject Line)
    {
        //实例化需要显示的线段图片pfb
        RectTransform rect = Line.GetComponent<RectTransform>();
        //设置位置和角度
        rect.localPosition = GetBetweenPoint(start, end);
        rect.localRotation = Quaternion.AngleAxis(-GetAngle(start, end), Vector3.forward);
        //设置线段图片大小
        var distance = Vector2.Distance(end, start);
        rect.sizeDelta = new Vector2(5, Mathf.Max(1, distance));
        //调整显示层级
        Line.transform.SetAsFirstSibling();
    }
    public float GetAngle(Vector2 start, Vector2 end)
    {
        var dir = end - start;
        var dirV2 = new Vector2(dir.x, dir.y);
        var angle = Vector2.SignedAngle(dirV2, Vector2.down);
        return angle;
    }
    private Vector2 GetBetweenPoint(Vector2 start, Vector2 end)
    {
        //两点之间垂直距离
        float distance = end.y - start.y;
        float y = start.y + distance / 2;
        float x = start.x;

        if (start.x != end.x)
        {
            //斜率值
            float k = (end.y - start.y) / (end.x - start.x);
            //根据公式 y = kx + b ， 求b
            float b = start.y - k * start.x;
            x = (y - b) / k;
        }

        Vector2 point = new Vector2(x, y);
        return point;
    }

}
