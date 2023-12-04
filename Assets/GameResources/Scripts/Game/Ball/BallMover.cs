using Game.Ball;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : BallComponent {
    /* Fields */
    [SerializeField] Rigidbody2D rb;

    Vector2 vel;

    //-------------------------------------------------------------------
    /* Properties */

    //-------------------------------------------------------------------
    /* Messages */
    protected override void Awake()
    {
        base.Awake();

        core.OnHit += HitProcess;
    }

    protected override void OnStart()
    {
        base.OnStart();

        vel = core.Direction * core.MoveSpeed;
        rb.velocity = vel;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        vel = rb.velocity;
    }

    //-------------------------------------------------------------------
    /* Methods */
    void HitProcess(Collision2D collision)
    {
        // ÉoÅ[ÇæÇ¡ÇΩÇÁî≤ÇØÇÈ
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bar")) return;

        var normal = collision.contacts[0].normal;

        var dir = Vector2.Reflect(vel, normal).normalized;
        core.SetDirection(dir);

        vel = core.Direction * core.MoveSpeed;
        rb.velocity = vel;
    }
}
