using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class AmmoScript : MonoBehaviour {
    protected Rigidbody2D rb;
    protected PlayerController owner;
    public int damage = 10;

    public PlayerController Owner {
        set {
            if (value != null) 
                owner = value;
        }
    }

    public virtual Vector3 LeftDisFromStart => new Vector3(0.7f, 0.7f, 0);
    public virtual Vector3 RightDisFromStart => new Vector3(-0.7f, 0.7f, 0);

    public Vector2 Velocity {
        set => rb.velocity = value;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Tree")) {
            rb.velocity = Vector2.zero;
            foreach (ContactPoint2D contact in other.contacts) {
                rb.AddForce(contact.normal * 100);
            }
            
            return;
        }
        Destroy(gameObject);
        if (other.gameObject.tag.Equals("Character")) {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            pc.Health -= damage;
        }
        GameManager.Instance.DoAction();
    }

    private void FixedUpdate() {
        rb.AddForce(GameManager.Instance.Wind);
    }
}