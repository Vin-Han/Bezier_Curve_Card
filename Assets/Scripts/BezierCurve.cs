using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    private List<Vector2> PointList = new List<Vector2>();  // ��¼���������ߵ�
    #region �ɰ�ģ��
    // ���ģ�ͳ�������е��񣬵���ת�ǲ�̫�ԣ�������
    // ��ȡ����������ÿ���ڵ�
    public List<Vector2> GetBezierPoint(Vector2 Cursor, int count, out Vector2 baseLookAt)
    {
        PointList.Clear();
        // ���������ߵ����������ߺͽ���
        Vector2 intersection = Intersection(Cursor, out baseLookAt);
        Vector2 line1 = Cursor;
        Vector2 line2 = intersection;
        // ԭ����յ�
        PointList.Add(Cursor);
        // ��������ֵ
        Vector2 gap = intersection - Cursor;
        Vector2 gap1 = new Vector2(gap.x / count, gap.y / count);
        Vector2 gap2 = new Vector2(intersection.x / count, intersection.y / count);
        // ����ڵ�ֵ
        for (int i = 1; i < count - 1; i++)
        {
            line1 += gap1;
            line2 -= gap2;
            Vector2 point = (line2 - line1) * (i / (float)count) + line1;
            PointList.Add(point);
        }
        PointList.Add(Vector2.zero);
        return PointList;
    }
    // ��ȡ���������߽���
    private Vector2 Intersection(Vector2 Cursor, out Vector2 firstLook)
    {
        // ��ʼ�㳯��
        float result = Cursor.x >= 0 ? Mathf.Pow(Cursor.x, 1.1f) : -Mathf.Pow(-Cursor.x, 1.1f);
        firstLook = new Vector2(result, 0);
        // �����㳯��ͱ���
        float angle = Cursor.x > 0 ? -Vector2.Angle(Cursor, Vector2.up) : Vector2.Angle(Cursor, Vector2.up);
        float percent = -result / Mathf.Sin(angle * Mathf.PI / 180) * Mathf.Cos(angle * Mathf.PI / 180);
        // ����
        return new Vector2(percent * Mathf.Sin(angle * Mathf.PI / 180), percent * Mathf.Cos(angle * Mathf.PI / 180));
    }
    #endregion

    #region �°�ģ��
    // ��ȡ����������ÿ���ڵ�
    public List<Vector2> GetBezierCurve(Vector2 Cursor, Vector2 Origin, int count)
    {
        PointList.Clear();
        // ��ʼ�ڵ�ĳ���
        Vector2 curSorLookAt = GetFirstLookAt(Cursor, Origin);
        PointList.Add(curSorLookAt);
        // ԭ��
        PointList.Add(Cursor);

        // ���������ߵ����������ߺͽ���
        Vector2 intersection = GetIntersection(Cursor, Origin);
        // ��ȥ��ͷ��־λ�ú�ļ����,ͬʱ�� int ת float 
        float realCount = count - 2;
        // ��������ֵ
        Vector2 gap1 = (intersection - Cursor) / realCount;
        Vector2 gap2 = (Origin - intersection) / realCount;
        // ÿ�α��������ߵ�����
        Vector2 base1 = Cursor;
        Vector2 base2 = intersection;
        // ��β��뱴�������ߵ�
        for (int i = 1; i < realCount; i++)
        {
            base1 += gap1;
            base2 += gap2;
            Vector2 point = (base2 - base1) * (i / realCount) + base1;
            PointList.Add(point);
        }

        // �յ�
        PointList.Add(Origin);
        return PointList;
    }
    // ͨ������Բ�����꣬��ȡ���˳�ʼ��ĳ���
    public Vector2 GetFirstLookAt(Vector2 Cursor,Vector2 Origin)
    {
        // ���ʹ�ֱ����н�
        float angle = Vector2.Angle(Cursor - Origin, Vector2.up);
        // ����ָ����
        float cursorAngle = 90 * Mathf.Cos((90 + angle) * Mathf.PI / 180);                                                              // ����ԶС
        float cursorX = Cursor.x > Origin.x ? Mathf.Sin(cursorAngle * Mathf.PI / 180) : -Mathf.Sin(cursorAngle * Mathf.PI / 180);      // X �� �ڰ�
        float cursorY = Cursor.y > Origin.y ? Mathf.Cos(cursorAngle * Mathf.PI / 180) : -Mathf.Cos(cursorAngle * Mathf.PI / 180);      // Y �� ��͹
        return new Vector2(Cursor.x - cursorX, Cursor.y - cursorY);

        #region �ɰ�ģ�ʹ������ģ�Ҳ�����㷨��֤������Ӧ����ʱ�ò���
        // ԭ���ָ����
        float originAngle = angle <= 90 ? angle * Mathf.Sin(angle * Mathf.PI / 180) : 180 - (180 - angle) * Mathf.Sin(angle * Mathf.PI / 180);  // ����ԶС
        float originX = Cursor.x > Origin.x ? -Mathf.Sin(originAngle * Mathf.PI / 180) : Mathf.Sin(originAngle * Mathf.PI / 180);
        float originY = Mathf.Cos(originAngle * Mathf.PI / 180);
        Vector2 originLookAt = new Vector2(Origin.x + originX, Origin.y + originY);
        #endregion
    }
    // ��ȡ���������ߵĽ����
    private Vector2 GetIntersection(Vector2 Cursor, Vector2 Origin)
    {
        // ��ȡ ����ΪԲ��
        Vector2 circleCenter = (Cursor + Origin) / 2;
        // ����ˮƽ����н�
        float horangle = Cursor.x > Origin.x ? Vector2.Angle(Cursor - Origin, Vector2.right) : Vector2.Angle(Cursor - Origin, Vector2.left); 
        // ���ʹ�ֱ����н�
        float verangle = Cursor.y > Origin.y ? Vector2.Angle(Cursor - Origin, Vector2.up) : Vector2.Angle(Cursor - Origin, Vector2.down);
        // ���������߽�����Բ�İ�ֱȵ� 
        float precent =  Mathf.Sin(verangle * Mathf.PI / 180);
        // ����ת�Ƕ�
        float angle = horangle + 180 * precent;
        // ��ȡ�����
        float cursorX = Cursor.x > Origin.x ? - Mathf.Cos(angle * Mathf.PI / 180) : Mathf.Cos(angle * Mathf.PI / 180);
        float cursorY = Cursor.y > Origin.y ? - Mathf.Sin(angle * Mathf.PI / 180) : Mathf.Sin(angle * Mathf.PI / 180);
        // �뾶ϵ��                                                         // ɱ¾����Ӧ����������������ģ��뾶ϵ���޸�Ӱ��������ߵ��۽Ǵ��������Ǹ�������ֵ
        float radius = Vector2.Distance(Cursor, Origin) / 2 * 2f;
        return new Vector2(circleCenter.x - radius * cursorX, circleCenter.y - radius * cursorY);
    }
    #endregion
}
