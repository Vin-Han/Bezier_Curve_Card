using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action upEvent;
    public Action downEvent;
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        downEvent?.Invoke();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        upEvent?.Invoke();
    }
}
