using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Egg : MonoBehaviour
{
    private Rigidbody2D rig;
    [SerializeField] private float bounceVelocity;
    private bool isAlive = true;
    private float gravityScale;

    [Header("Events")]
    public static Action onHit;
    public static Action onFellInWater;

    void Start() {
        rig = GetComponent<Rigidbody2D>();
        isAlive=true;

        gravityScale = rig.gravityScale;
        rig.gravityScale = 0;

        StartCoroutine("WaitAndFall");
    }

    IEnumerator WaitAndFall(){
        yield return new WaitForSeconds(2);

        rig.gravityScale = gravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider.TryGetComponent(out PlayerController playerController)){
            Bounce(collision.GetContact(0).normal); //contact 0 es el primer contacto
            playerController.Bump();
            onHit?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        
        if(!isAlive){
            return;
        }
        
        if(collider.CompareTag("Water")){   
            isAlive=false;      
            onFellInWater?.Invoke();   
        }
    }

    private void Bounce(Vector2 normal){
        rig.velocity = normal * bounceVelocity;
    }

    public void Reuse(){
        transform.position = Vector2.up * 5;
        rig.velocity = Vector2.zero;
        rig.angularVelocity = 0;
        rig.gravityScale = 0;
        transform.rotation = Quaternion.Euler(0,0,0);

        isAlive=true;

        StartCoroutine("WaitAndFall");
    }
}
