using System.Collections;
using System.Collections.Generic;
using Unity.Labs.SuperScience;
using UnityEngine;

public class RemoveItems : MonoBehaviour
{
    public void DestroyStuff()
    {
        Destroy(gameObject, 3f);
    }
}
