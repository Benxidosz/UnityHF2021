using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Transform _playerTrans;
    private AmmoScript _ammo;
    private bool _left;
    private int _health;
    public event Action HealthChanged;
    private AmmoScript _nextSpecial = null;

    public AmmoScript NextSpec {
        set => _nextSpecial = value;
    }
    

    public int Health {
        get => _health;
        set {
            if (value < 0)
                _health = 0;
            else if (value > GameManager.Instance.maxHeath)
                _health = GameManager.Instance.maxHeath;
            else
                _health = value;
            HealthChanged?.Invoke();
        }
    }

    public bool Left {
        set => _left = value;
    }
    
    public AmmoScript Ammo {
        set => _ammo = value;
    }

    private void Awake() {
        _playerTrans = GetComponent<Transform>();
    }
    
    public virtual void Shoot() {
        
    }

    public void Shoot(Vector2 velocity) {
        AmmoScript ammo;
        if (_nextSpecial is null) {
            ammo = Instantiate(_ammo,
                _playerTrans.position +
                (_left ? _ammo.LeftDisFromStart : _ammo.RightDisFromStart)
                , Quaternion.Euler(Vector3.zero));
        }
        else {
            ammo = Instantiate(_nextSpecial,
                _playerTrans.position +
                (_left ? _nextSpecial.LeftDisFromStart : _nextSpecial.RightDisFromStart)
                , Quaternion.Euler(Vector3.zero));
        }
        
        ammo.Velocity = velocity;
        ammo.Owner = this;
    }

    public Vector3 GetAimPos(double alfa, double r) {
        if (!_left)
            alfa += 90;

        double rad = alfa * Math.PI / 180;
        
        Vector3 centerPos = _playerTrans.position;
        double x = centerPos.x + r * Math.Cos(rad);
        double y = centerPos.y + r * Math.Sin(rad);
        
        return new Vector3((float) x,(float) y, 0);
    }

    public virtual void takeTurn() {
        
    }
}