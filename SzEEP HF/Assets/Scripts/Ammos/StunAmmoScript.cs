using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class StunAmmoScript : AmmoScript {
    protected override void OnCollisionEnter2D(Collision2D other) {
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
            GameManager.Instance.Stun();
        } else 
            GameManager.Instance.DoAction();
        owner.NextSpec = null;
    }
}
