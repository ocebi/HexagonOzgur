using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static Action<List<GridObject>> OnClickDetected;
    public static Action<List<GridObject>, Vector3, Vector3> OnSwipeDetected;

    private bool clickAvailable = true;
    private Vector3? clickStartPosition;
    private Vector3? clickEndPosition;

    private void OnEnable()
    {
        ExplosionManager.OnExplosionCompleted += SetClickAvailable;
        GroupSelectorController.OnRotationCompleted += SetClickAvailable;
        GameManager.OnGameOver += SetClickUnavailable;
    }

    private void OnDisable()
    {
        ExplosionManager.OnExplosionCompleted -= SetClickAvailable;
        GroupSelectorController.OnRotationCompleted -= SetClickAvailable;
        GameManager.OnGameOver -= SetClickUnavailable;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickAvailable && Input.GetMouseButtonDown(0))
        {
            //CheckInput();
#if UNITY_EDITOR
            clickStartPosition = Input.mousePosition;
#else
            Touch touch = Input.GetTouch(0);
            clickStartPosition = touch.position;
#endif
        }

        if (clickAvailable && Input.GetMouseButtonUp(0))
        {
#if UNITY_EDITOR
            clickEndPosition = Input.mousePosition;
#else
            Touch touch = Input.GetTouch(0);
            clickEndPosition = touch.position;
#endif
            //clickEndPosition = Input.mousePosition;
            CheckInput();
        }
    }

    private void CheckInput()
    {
        //Click

        if (Vector3.Distance(clickStartPosition.Value, clickEndPosition.Value) <= 10f)
        {
            RaycastHit2D hit = GetHitPoint();
            if (hit.collider != null)
            {
                GridObject hitObject = hit.transform.GetComponent<GridObject>();
                if (!hitObject) return;
                var angle = CalculateAngle(hitObject.transform.position);
                var groupSelectorCenter = GetGroupCenter(hitObject, angle);
                if(groupSelectorCenter == null)
                {
                    Debug.Log("Invalid location.");
                    return;
                }

                List<GridObject> gridObjectGroup = GridManager.Instance.GetGroupAt(groupSelectorCenter.Value);
                if (gridObjectGroup != null)
                {
                    OnClickDetected.Invoke(gridObjectGroup);
                }
                else
                    Debug.Log("Invalid click");

                //List<GridObject> gridObjectGroup = GridManager.Instance.GetGroupAt(hit.point);
                //if (gridObjectGroup != null)
                //{
                //    OnClickDetected.Invoke(gridObjectGroup);
                //}
                //else
                //    Debug.Log("Invalid click");
            }
            else
                Debug.Log("Hit collider is null");
        }
        else
        {
            //clickEndPosition = null;
            Debug.Log("Swipe detected.");
            List<GridObject> gridObjectGroup = GridManager.Instance.GetGroupAtSelector();
            if (gridObjectGroup == null) return;
            OnSwipeDetected?.Invoke(gridObjectGroup, clickStartPosition.Value, clickEndPosition.Value);
            clickAvailable = false;
            //Invoke(nameof(EndSwipe), 1f);
        }
    }

    private void SetClickAvailable()
    {
        clickAvailable = true;
    }

    private void SetClickUnavailable()
    {
        clickAvailable = false;
    }

    private RaycastHit2D GetHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(clickStartPosition.Value);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("GridObject"));
        
        return hit;
    }

    private float CalculateAngle(Vector3 hitCenter)
    {
        Vector3 clickPos = clickStartPosition.Value;
        clickPos.z = hitCenter.z - Camera.main.transform.position.z;
        clickPos = Camera.main.ScreenToWorldPoint(clickPos);
        clickPos = clickPos - hitCenter;
        var angle = Mathf.Atan2(clickPos.y, clickPos.x) * Mathf.Rad2Deg;
        if (angle < 0.0f) angle += 360.0f;
        Debug.Log("Angle: " + angle);
        return angle;
    }

    private Vector2? GetGroupCenter(GridObject hitObject, float angle)
    {
        hitObject.neighbors = hitObject.GetNeighbors();

        if (angle >= 315 || angle < 45)
        {
            //right
            Debug.Log("Right");
            return new Vector2(hitObject.transform.position.x + GridObject.HALF_VERTICAL, hitObject.transform.position.y);
        }
        else if (angle >= 45 && angle < 90)
        {
            //upRight
            Debug.Log("upRight");
            return hitObject.neighbors.upRight + new Vector2(-GridObject.HALF_VERTICAL, 0);
        }
        else if (angle >= 90 && angle < 135)
        {
            //upLeft
            Debug.Log("upLeft");
            return hitObject.neighbors.upLeft + new Vector2(GridObject.HALF_VERTICAL, 0);
        }
        else if (angle >= 135 && angle < 225)
        {
            //left
            Debug.Log("left");
            return new Vector2(hitObject.transform.position.x - GridObject.HALF_VERTICAL, hitObject.transform.position.y);
        }
        else if (angle >= 225 && angle < 275)
        {
            //downLeft
            Debug.Log("downLeft");
            return hitObject.neighbors.downLeft + new Vector2(GridObject.HALF_VERTICAL, 0);
        }
        else if (angle >= 275 && angle < 315)
        {
            //downRight
            Debug.Log("downRight");
            return hitObject.neighbors.downRight + new Vector2(-GridObject.HALF_VERTICAL, 0);
        }
        else
        {
            Debug.Log("Couldn't find the center");
            return null;
        }
    }
}
