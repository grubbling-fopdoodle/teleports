﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController {
	
	// Update is called once per frame
	public override void Control () {
        if (Input.GetButton("PlayerMove"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool done = false;

            int enemyMask = (int)LayerMask.Enemy;
            int groundMask = (int)LayerMask.Ground;
            if (Input.GetButtonDown("PlayerMove") && Physics.Raycast(ray, out hit, Mathf.Infinity, enemyMask))
            {
                OnEnemyClick(hit.transform.gameObject.GetComponent<Unit>());
            }
            else if (!done && Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                OnGroundPressed(hit.point);
            }
        }

        if (target.TargetUnit != null)
        {
            if (!target.TargetUnit.Alive || !unit.Alive) target.TargetUnit = null;
            else Chase();
        }
	}  
    
    void OnEnemyClick(Unit enemy)
    {
        target.TargetUnit = enemy;
    }  

    void OnGroundPressed(Vector3 point)
    {
        unit.MovingState.Start(point);
    }
}
