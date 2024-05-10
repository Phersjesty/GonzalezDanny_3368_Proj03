using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float moveSpeed = 1;



    private void Update()
    {
        
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
        transform.Translate(new Vector3(movement.x, movement.y, 0));
       
    }
}
