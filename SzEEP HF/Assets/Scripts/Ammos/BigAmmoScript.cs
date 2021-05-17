using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAmmoScript : AmmoScript {
    public override Vector3 LeftDisFromStart => new Vector3(1f, 1f, 0);
    public override Vector3 RightDisFromStart => new Vector3(-1f, 1f, 0);

    protected override void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        owner.NextSpec = null;
    }
}
