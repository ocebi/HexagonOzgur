using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSelectorController : MonoBehaviour
{
    public static Action OnRotationCompleted;

    private int rotationStage = 0;
    private List<GridObject> currentGroup;
    private List<Vector3> initialPositions;
    private int swipeDirection = 1;

    #region Unity Methods
    private void OnEnable()
    {
        GameInputManager.OnSwipeDetected += StartRotation;
        rotationStage = 0;
    }

    private void OnDisable()
    {
        GameInputManager.OnSwipeDetected -= StartRotation;
    }
    #endregion


    #region Public Methods
    /// <summary>
    /// Sets the layout of the group selector.
    /// </summary>
    /// <param name="layout"> Layout is either 1 or -1. </param>
    public void SetLayout(int layout)
    {
        transform.rotation = Quaternion.Euler(0, 0, -60f * layout);
    }

    /// <summary>
    /// Sets the position of the group selector.
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        position.z = transform.position.z;
        transform.position = position;
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes the variables and starts the rotation of the group.
    /// </summary>
    /// <param name="group"> Group to rotate. </param>
    /// <param name="start"> Swipe start position. </param>
    /// <param name="end"> Swipe end position. </param>
    private void StartRotation(List<GridObject> group, Vector3 start, Vector3 end)
    {
        currentGroup = group;
        initialPositions = new List<Vector3>();
        foreach(var go in group)
        {
            initialPositions.Add(go.transform.position);
        }
        SetGroupTransform(true);
        SetSwipeDirection(start, end);
        RotateGroup();
    }

    private void RotateGroup()
    {
        transform.DORotate(new Vector3(0, 0, 120 * swipeDirection), 0.2f, RotateMode.WorldAxisAdd).OnComplete(RotationCheck);
    }

    /// <summary>
    /// Checks if the current position of the group explodes or the rotation has completed.
    /// Rotates the group until the rotation is completed.
    /// </summary>
    private void RotationCheck()
    {
        Debug.Log("Inside rotation check");
        SetGroupTransform(false);
        GroupPositionCorrection();
        
        if(rotationStage < 2)
        {
            var explosion = ExplosionManager.Instance.CheckExplosion(currentGroup);
            if (explosion)
            {
                Debug.Log("Explosion");
                gameObject.SetActive(false);
                this.enabled = false;
                return;
            }
            else
            {
                SetGroupTransform(true);
                RotateGroup();
            }
            ++rotationStage;
        }
        else
        {
            rotationStage = 0;
            OnRotationCompleted.Invoke();
        }
    }

    /// <summary>
    /// Sets the parent of the group elements to group selector or grid.
    /// </summary>
    private void SetGroupTransform(bool selected)
    {
        if(selected)
        {
            currentGroup[0].transform.parent = transform;
            currentGroup[1].transform.parent = transform;
            currentGroup[2].transform.parent = transform;
        }
        else
        {
            currentGroup[0].transform.parent = GridGenerator.Instance.transform;
            currentGroup[1].transform.parent = GridGenerator.Instance.transform;
            currentGroup[2].transform.parent = GridGenerator.Instance.transform;
        }
    }

    /// <summary>
    /// Calculates the direction to rotate the objects based on the swipe input.
    /// </summary>
    /// <param name="start"> Swipe start position. </param>
    /// <param name="end"> Swipe end position. </param>
    private void SetSwipeDirection(Vector3 start, Vector3 end)
    {
        if(Mathf.Abs(end.x - start.x) >= Math.Abs(end.y - start.y)) //horizontal swipe
        {
            int upOrDown;
            var clickPos = Camera.main.ScreenToWorldPoint(start);
            if (clickPos.y <= transform.position.y)
                upOrDown = 1;
            else
                upOrDown = -1;

            if (end.x - start.x >= 0) //right swipe
            {
                swipeDirection = upOrDown;
            }
            else //left swipe
            {
                swipeDirection = -upOrDown;
            }
            
        }
        else  //vertical swipe
        {
            int leftOrRight;
            var clickPos = Camera.main.ScreenToWorldPoint(start);
            if (clickPos.x <= transform.position.x)
                leftOrRight = -1;
            else
                leftOrRight = 1;
            if (end.y - start.y >= 0) //up swipe
            {
                swipeDirection = leftOrRight;
            }
            else //down swipe
            {
                swipeDirection = -leftOrRight;
            }

            //swipeDirection = -1;
        }
    }

    /// <summary>
    /// Corrects the group position by rounding the positions.
    /// </summary>
    private void GroupPositionCorrection()
    {
        foreach(var go in currentGroup)
        {
            Vector3 position = go.transform.position;

            foreach (var vec in initialPositions)
            {
                if (isPositionEqual(position, vec, 0.1f))
                {
                    go.transform.position = vec;
                    break;
                }
            }

            GridManager.RoundPosition(go);
            GridManager.RoundScale(go);
        }
    }

    /// <summary>
    /// Checks if the provided positions are equal or close.
    /// </summary>
    private bool isPositionEqual(Vector2 pos1, Vector2 pos2, float epsilon)
    {
        if (nearlyEqual(pos1.x, pos2.x, epsilon) && nearlyEqual(pos1.y, pos2.y, epsilon))
            return true;
        else
            return false;
    }

    private bool nearlyEqual(float a, float b, float epsilon)
    {
        float absA = Mathf.Abs(a);
        float absB = Mathf.Abs(b);
        float diff = Mathf.Abs(a - b);

        if (a == b)
        { // shortcut, handles infinities
            return true;
        }
        else if (a == 0 || b == 0 || absA + absB < float.MinValue)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < (epsilon * float.MinValue);
        }
        else
        { // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
    #endregion
}
