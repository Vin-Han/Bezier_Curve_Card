using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierLine : MonoBehaviour
{
    public ButtonClick beginBTN;         // ������ť
    public InputField countInput;        // ��������

    public RectTransform Sencor;    // ���������ж����λ�õĶ���
    public Image HeadImage;         // ͷ��ͼ��
    public Image BaseImage;         // ����ͼ��

    private bool ifWorking = false; // �Ƿ��ڹ���
    private int pointCount = 10;    // �����ڵ�����

    private Vector3 basePos;        // ԭ������
    private Vector3 biasPos;        // ������꣨��ԭ��ͳһ����ϵʱ

    private BezierCurve bezierCurve = new BezierCurve();    // ���������߼�����
    private List<RectTransform> transList = new List<RectTransform>();      // �������������
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
    // ��ʼ�����е�����ڵ�ͱ�Ҫ����
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
    // ������������ڵ��λ�ø�ԭ
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
    // �����еı������ڵ�
    private void DrawBezierPoint(Vector3 mousePos)
    {
        Vector3 relativePos = mousePos - biasPos;
        List<Vector2> pointList = bezierCurve.GetBezierCurve(relativePos, basePos, transList.Count);
        transList[0].localPosition = pointList[0];      // ��־�ﲻת��
        for (int i = 1; i < transList.Count; i++)
        {
            transList[i].localPosition = pointList[i];
            transList[i].up = (transList[i - 1].position - transList[i].position).normalized;
        }
    }





    /// ����һ������֮�����
    public void CreateLine(Vector2 start, Vector2 end, GameObject Line)
    {
        //ʵ������Ҫ��ʾ���߶�ͼƬpfb
        RectTransform rect = Line.GetComponent<RectTransform>();
        //����λ�úͽǶ�
        rect.localPosition = GetBetweenPoint(start, end);
        rect.localRotation = Quaternion.AngleAxis(-GetAngle(start, end), Vector3.forward);
        //�����߶�ͼƬ��С
        var distance = Vector2.Distance(end, start);
        rect.sizeDelta = new Vector2(5, Mathf.Max(1, distance));
        //������ʾ�㼶
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
        //����֮�䴹ֱ����
        float distance = end.y - start.y;
        float y = start.y + distance / 2;
        float x = start.x;

        if (start.x != end.x)
        {
            //б��ֵ
            float k = (end.y - start.y) / (end.x - start.x);
            //���ݹ�ʽ y = kx + b �� ��b
            float b = start.y - k * start.x;
            x = (y - b) / k;
        }

        Vector2 point = new Vector2(x, y);
        return point;
    }

}
