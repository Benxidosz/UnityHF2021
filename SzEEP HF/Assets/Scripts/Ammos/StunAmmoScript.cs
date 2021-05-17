using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class StunAmmoScript : AmmoScript {
    protected override void OnCollisionEnter2D(Collision2D other) {
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
