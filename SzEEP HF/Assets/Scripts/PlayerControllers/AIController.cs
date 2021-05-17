using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIController : PlayerController {
    private float _waited = 0;
    private float _needToWaited = 2;
    private Animator _animator;
    private bool _myTurn = false;
    
    private void Start() {
        _animator = GetComponent<Animator>();
    }

    public override void takeTurn() {
        _waited = 0;
        _needToWaited = Random.Range(1.0f, 3.0f);
        _myTurn = true;
        _animator.Play("PrepareShoot");
    }

    public override void Shoot() {
        double deg = Random.Range(105, 180);
        double rad = deg * Math.PI / 180;
        double x = transform.position.x + GameManager.Instance.r * Math.Cos(rad);
        double y = transform.position.y + GameManager.Instance.r * Math.Sin(rad);
        Vector2 v = new Vector2((float) x - transform.position.x, (float) y - transform.position.y);
        _animator.Play("Shoot");
        base.Shoot(v.normalized * Random.Range(GameManager.Instance.minForce, GameManager.Instance.maxForce));
        GameManager.Instance.DoAction();
    }

    private void Update() {
        if (_myTurn) {
            _waited += Time.deltaTime;
            if (_waited >= _needToWaited) {
                Shoot();
                _myTurn = false;
            }
        }
    }
}
