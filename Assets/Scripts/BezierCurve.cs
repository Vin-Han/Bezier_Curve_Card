using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    private List<Vector2> PointList = new List<Vector2>();  // 记录贝塞尔曲线点
    #region 旧版模型
    // 这个模型出来结果有点像，但是转角不太对，重做了
    // 获取贝塞尔曲线每个节点
    public List<Vector2> GetBezierPoint(Vector2 Cursor, int count, out Vector2 baseLookAt)
    {
        PointList.Clear();
        // 贝塞尔曲线的两条基础线和交点
        Vector2 intersection = Intersection(Cursor, out baseLookAt);
        Vector2 line1 = Cursor;
        Vector2 line2 = intersection;
        // 原点和终点
        PointList.Add(Cursor);
        // 计算区间值
        Vector2 gap = intersection - Cursor;
        Vector2 gap1 = new Vector2(gap.x / count, gap.y / count);
        Vector2 gap2 = new Vector2(intersection.x / count, intersection.y / count);
        // 插入节点值
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
    // 获取贝塞尔曲线交点
    private Vector2 Intersection(Vector2 Cursor, out Vector2 firstLook)
    {
        // 初始点朝向
        float result = Cursor.x >= 0 ? Mathf.Pow(Cursor.x, 1.1f) : -Mathf.Pow(-Cursor.x, 1.1f);
        firstLook = new Vector2(result, 0);
        // 结束点朝向和比例
        float angle = Cursor.x > 0 ? -Vector2.Angle(Cursor, Vector2.up) : Vector2.Angle(Cursor, Vector2.up);
        float percent = -result / Mathf.Sin(angle * Mathf.PI / 180) * Mathf.Cos(angle * Mathf.PI / 180);
        // 交点
        return new Vector2(percent * Mathf.Sin(angle * Mathf.PI / 180), percent * Mathf.Cos(angle * Mathf.PI / 180));
    }
    #endregion

    #region 新版模型
    // 获取贝塞尔曲线每个节点
    public List<Vector2> GetBezierCurve(Vector2 Cursor, Vector2 Origin, int count)
    {
        PointList.Clear();
        // 初始节点的朝向
        Vector2 curSorLookAt = GetFirstLookAt(Cursor, Origin);
        PointList.Add(curSorLookAt);
        // 原点
        PointList.Add(Cursor);

        // 贝塞尔曲线的两条基础线和交点
        Vector2 intersection = GetIntersection(Cursor, Origin);
        // 减去开头标志位置后的间隔数,同时把 int 转 float 
        float realCount = count - 2;
        // 计算区间值
        Vector2 gap1 = (intersection - Cursor) / realCount;
        Vector2 gap2 = (Origin - intersection) / realCount;
        // 每次贝塞尔曲线的两端
        Vector2 base1 = Cursor;
        Vector2 base2 = intersection;
        // 逐段插入贝塞尔曲线点
        for (int i = 1; i < realCount; i++)
        {
            base1 += gap1;
            base2 += gap2;
            Vector2 point = (base2 - base1) * (i / realCount) + base1;
            PointList.Add(point);
        }

        // 终点
        PointList.Add(Origin);
        return PointList;
    }
    // 通过鼠标和圆心坐标，获取两端初始点的朝向
    public Vector2 GetFirstLookAt(Vector2 Cursor,Vector2 Origin)
    {
        // 鼠标和垂直方向夹角
        float angle = Vector2.Angle(Cursor - Origin, Vector2.up);
        // 鼠标的指向方向
        float cursorAngle = 90 * Mathf.Cos((90 + angle) * Mathf.PI / 180);                                                              // 近大远小
        float cursorX = Cursor.x > Origin.x ? Mathf.Sin(cursorAngle * Mathf.PI / 180) : -Mathf.Sin(cursorAngle * Mathf.PI / 180);      // X 轴 内凹
        float cursorY = Cursor.y > Origin.y ? Mathf.Cos(cursorAngle * Mathf.PI / 180) : -Mathf.Cos(cursorAngle * Mathf.PI / 180);      // Y 轴 外凸
        return new Vector2(Cursor.x - cursorX, Cursor.y - cursorY);

        #region 旧版模型带过来的，也做了算法验证，但是应该暂时用不着
        // 原点的指向方向
        float originAngle = angle <= 90 ? angle * Mathf.Sin(angle * Mathf.PI / 180) : 180 - (180 - angle) * Mathf.Sin(angle * Mathf.PI / 180);  // 近大远小
        float originX = Cursor.x > Origin.x ? -Mathf.Sin(originAngle * Mathf.PI / 180) : Mathf.Sin(originAngle * Mathf.PI / 180);
        float originY = Mathf.Cos(originAngle * Mathf.PI / 180);
        Vector2 originLookAt = new Vector2(Origin.x + originX, Origin.y + originY);
        #endregion
    }
    // 获取贝塞尔曲线的交叉点
    private Vector2 GetIntersection(Vector2 Cursor, Vector2 Origin)
    {
        // 获取 两点为圆心
        Vector2 circleCenter = (Cursor + Origin) / 2;
        // 鼠标和水平方向夹角
        float horangle = Cursor.x > Origin.x ? Vector2.Angle(Cursor - Origin, Vector2.right) : Vector2.Angle(Cursor - Origin, Vector2.left); 
        // 鼠标和垂直方向夹角
        float verangle = Cursor.y > Origin.y ? Vector2.Angle(Cursor - Origin, Vector2.up) : Vector2.Angle(Cursor - Origin, Vector2.down);
        // 贝塞尔曲线交点在圆的半分比点 
        float precent =  Mathf.Sin(verangle * Mathf.PI / 180);
        // 总旋转角度
        float angle = horangle + 180 * precent;
        // 获取交叉点
        float cursorX = Cursor.x > Origin.x ? - Mathf.Cos(angle * Mathf.PI / 180) : Mathf.Cos(angle * Mathf.PI / 180);
        float cursorY = Cursor.y > Origin.y ? - Mathf.Sin(angle * Mathf.PI / 180) : Mathf.Sin(angle * Mathf.PI / 180);
        // 半径系数                                                         // 杀戮尖塔应该是在这里有区别的，半径系数修改影响控制曲线的折角处，现在是个经验数值
        float radius = Vector2.Distance(Cursor, Origin) / 2 * 2f;
        return new Vector2(circleCenter.x - radius * cursorX, circleCenter.y - radius * cursorY);
    }
    #endregion
}
