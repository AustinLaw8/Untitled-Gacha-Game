using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Note : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 10f;    

    void Start()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
    
    // Note fall!
    void FixedUpdate()
    {
        this.transform.position = new Vector3(
                this.transform.position.x,
                this.transform.position.y - fallSpeed * Time.fixedDeltaTime, 
                this.transform.position.z
        );
    }
}
