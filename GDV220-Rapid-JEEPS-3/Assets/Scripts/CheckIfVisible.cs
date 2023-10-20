using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfVisible : MonoBehaviour
{
    public bool visible = false;

    private void OnBecameVisible()
    {
        visible = true;
    }
}
