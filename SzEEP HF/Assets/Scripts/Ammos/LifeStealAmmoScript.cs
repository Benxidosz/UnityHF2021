using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStealAmmoScript : AmmoScript {
    protected override void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        if (other.gameObject.tag.Equals("Character"))
            owner.Health += 30;
        owner.NextSpec = null;
    }
}
