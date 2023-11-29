using Game.Ball;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : BallComponent {
    /* Fields */
    [SerializeField] float minHitResult = .25f;
    [SerializeField] Vector2 moveDir = Vector2.left;

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

        moveDir.Normalize();
        vel = moveDir * core.MoveSpeed;
        rb.velocity = vel;
    }

    private void Update()
    {
        vel = rb.velocity;
    }

    //-------------------------------------------------------------------
    /* Methods */
    void HitProcess(Collision2D collision)
    {
        // ÉoÅ[ÇæÇ¡ÇΩÇÁî≤ÇØÇÈ
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bar")) return;

        var normal = collision.contacts[0].normal;
        moveDir = Vector2.Reflect(vel, normal).normalized;

        vel = moveDir * core.MoveSpeed;
        rb.velocity = vel;

        print($"ball magnitude: {rb.velocity.magnitude}");
    }
}
