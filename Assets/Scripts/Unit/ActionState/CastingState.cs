﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingState : ActionState {

    protected Skill.TargetInfo castTarget;
    protected Skill activeSkill;
    protected float currentCastTime;

    public event EventHandler startCastEvent, castEvent, resetCastEvent;

    public CastingState(Unit unit) : base(unit)
    {
        Reset();
    }

    public override void Start()
    {
        /*Debug.Log(
            "castTarget: " + CastTarget.TargetUnit.name +
            " ||| activeSkill: " + activeSkill.Name +
            " ||| current cooldown: " + activeSkill.CurrentCooldown.ToString() +
            " ||| can reach cast target?: " + CanReachCastTarget.ToString());*/
        if(!IsActive && castTarget != null && activeSkill != null && activeSkill.CurrentCooldown == 0 && CanReachCastTarget)
        {
            //Debug.Log("Casting start");
            currentCastTime = 0;
            isActive = true;
            if (startCastEvent != null) startCastEvent(this, EventArgs.Empty);
        }
    }

    public override void Update(float dTime)
    {
        if (isActive && !IsBlocked && CanReachCastTarget)
        {
            currentCastTime += dTime;
            if (currentCastTime >= activeSkill.CastTime)
            {
                activeSkill.Cast(unit, castTarget);
                if (castEvent != null) castEvent(this, EventArgs.Empty);
                Reset();
            }
        }
        else if(isActive)
        {
            Reset();
        }
    }

    public override void Reset()
    { 
        castTarget = null;
        activeSkill = null;
        currentCastTime = 0;
        isActive = false;
        if (resetCastEvent != null) resetCastEvent(this, EventArgs.Empty);
    }

    public void Start(Skill skill, Skill.TargetInfo target, bool interrupt = false)
    {
        if (IsActive)
        {
            if (interrupt)
                Reset();
            else
                return;
        }
        activeSkill = skill;
        castTarget = target;
        Start();
    }

    bool CanReachCastTarget
    {
        get
        {
            return activeSkill.CanReachCastTarget(castTarget);
        }
    }

    Skill.TargetInfo CastTarget
    {
        get { return castTarget; }
    }

    Skill ActiveSkill
    {
        get { return activeSkill; }
    }

}
