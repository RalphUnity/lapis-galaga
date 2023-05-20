using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVFX : MonoBehaviour
{

    public float destroyTime = 3f;
    private void OnEnable()
    {
        Invoke("HideVFX", 3f);
    }

    public void HideVFX()
    {
        gameObject.SetActive(false);
    }
}
