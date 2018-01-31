﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Skill : MonoBehaviour, IUniqueName {
       
    [FormerlySerializedAs("name_"), SerializeField] new private string name;
    [FormerlySerializedAs("type_"), SerializeField] private TargetType type;
    [FormerlySerializedAs("reach_"), SerializeField] private Attribute reach;
    [FormerlySerializedAs("cooldown_"), SerializeField] private Attribute cooldown;
    [FormerlySerializedAs("castTime_"), SerializeField] private Attribute castTime;
    [SerializeField] private Attribute totalCastTime;
    [SerializeField] private Attribute earlyBreakTime;
    [SerializeField] private SkillGraphics graphics;

    float currentCooldown;

    virtual public void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0) currentCooldown = 0;
    }
    
    abstract public void CastInternal(Unit caster, List<CastTarget> targets);

    protected virtual SkillTargeter GetTargeter()
    {
        return new SkillTargeter_Point();
    }

    public void Cast(Unit caster, TargetInfo targetInfo)
    {
        currentCooldown = cooldown.Value;
        CastInternal(caster, Targeter.GetTargets(this, targetInfo));
    }

    public bool CanReachTarget(TargetInfo targetInfo)
    {
        Unit caster = targetInfo.Caster;
        if (caster != null)
        {
            float totalReach = caster.Reach + caster.Size + Reach;
            if (targetInfo.TargetUnit != null) totalReach += targetInfo.TargetUnit.Size;

            return
                (targetInfo.Position - caster.transform.position).magnitude
                <=
                totalReach;
        }
        else
        {
            return false;
        }
    }

    public void ModifyAttribute(AttributeType type, float bonus, float multiplier)
    {
        GetAttribute(type).Modify(bonus, multiplier);
    }

    Attribute GetAttribute(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.Cooldown:
                return cooldown;
            case AttributeType.CastTime:
                return castTime;
            case AttributeType.TotalCastTime:
                return totalCastTime;
            case AttributeType.EarlyBreakTime:
                return earlyBreakTime;
            case AttributeType.Reach:
                return reach;
            default:
                return null;
        }
    }

    public string UniqueName
    {
        get { return name; }
    }

    public TargetType Type
    {
        get { return type; }
    }

    public float Reach
    {
        get { return reach.Value; }
    }

    public float Cooldown
    {
        get { return cooldown.Value; }
    }

    public float CastTime
    {
        get { return castTime.Value; }
    }

    public float TotalCastTime
    {
        get { return totalCastTime.Value; }
    }

    public float EarlyBreakTime
    {
        get { return earlyBreakTime.Value; }
    }

    public float CurrentCooldown
    {
        get { return currentCooldown; }
    }

    public SkillGraphics Graphics
    {
        get { return graphics; }
    }

    private SkillTargeter Targeter
    {
        get
        {
            return GetTargeter();
        }
    }

    public enum TargetType
    {
        Unit,
        Position
    };

    public enum AttributeType
    {
        Reach,
        Cooldown,
        CastTime,
        TotalCastTime,
        EarlyBreakTime
    }

    [System.Serializable]
    public class TargetInfo
    {
        Unit caster;
        TargetType targetType;
        Unit targetUnit;
        Vector3 targetPosition;

        public TargetInfo(Unit caster, Unit targetUnit)
            : this(caster, TargetType.Unit, targetUnit, Vector3.zero)
        { }

        public TargetInfo(Unit caster, Vector3 targetPosition)
            : this(caster, TargetType.Position, null, targetPosition)
        { }

        TargetInfo(Unit caster, TargetType targetType, Unit targetUnit, Vector3 targetPosition)
        {
            this.caster = caster;
            this.targetType = targetType;
            this.targetUnit = targetUnit;
            this.targetPosition = targetPosition;
        }

        public TargetInfo(TargetInfo other)
        {
            this.caster = other.caster;
            this.targetType = other.targetType;
            this.targetUnit = other.targetUnit;
            this.targetPosition = other.targetPosition;
        }

        public override bool Equals(object obj)
        {
            TargetInfo other = (TargetInfo)obj;
            if (obj == null) return false;
            else
            {
                return
                    caster == other.caster &&
                    targetType == other.targetType &&
                    targetUnit == other.targetUnit &&
                    targetPosition == other.targetPosition;
            }
        }

        public Unit Caster
        {
            get { return caster; }
        }

        public TargetType TargetType
        {
            get { return targetType; }
        }

        public Unit TargetUnit
        {
            get { return targetUnit; }
            set { targetUnit = value; }
        }

        public Vector3 Position
        {
            get
            {
                if (targetUnit != null)
                {
                    return TargetUnit.transform.position;
                }
                else return targetPosition;
            }
        }
    }
}
