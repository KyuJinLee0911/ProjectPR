using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    [SerializeField]
    GameObject characterObj;
    [SerializeField]
    Vector3 distVec;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = characterObj.transform.position + distVec;
    }
}
