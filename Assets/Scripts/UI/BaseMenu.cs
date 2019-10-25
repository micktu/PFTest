using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenu : MonoBehaviour
{
    public bool IsActive
    {
        get { return gameObject.activeSelf; }
    }

    public virtual void Init()
    {

    }

    public virtual void EnterMenu()
    {
        gameObject.SetActive(true);
    }

    public virtual void LeaveMenu()
    {
        gameObject.SetActive(false);
    }
}
