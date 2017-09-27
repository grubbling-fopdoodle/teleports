﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController : MonoBehaviour {

    protected Unit unit;
    protected Skill.TargetInfo target;
    [SerializeField]
    protected Skill mainAttack;

    public Skill.TargetInfo Target
    {
        set { target = value; }
    }

    public Skill MainAttack
    {
        get { return mainAttack; }
        set { mainAttack = value; }
    }

    public virtual void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
        target = new Skill.TargetInfo();
        if(mainAttack == null)
        {
            mainAttack = GetComponent<Skill>();
        }
    }

    void Update()
    {
        if (IsActive())
        {
            Control();
        }
    }

    protected void Chase()
    {
        if (unit.canReachCastTarget(mainAttack, target))
        {
            unit.Cast(mainAttack, target);
        }
        else
        {
            unit.moveTo(target.Position);
        }
    }

    public abstract void Control();

    bool IsActive()
    {
        return this == unit.ActiveController;
    }
}
