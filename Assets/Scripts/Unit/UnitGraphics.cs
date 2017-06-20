﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphics : MonoBehaviour {

    Healthbar healthbar_;
    GameObject targetMarker_;

	// Use this for initialization
	void Start () {
        healthbar_ = gameObject.AddComponent<Healthbar>();
	}

    public void updateTarget(Unit target)
    {
        if(targetMarker_ == null)
        {
            targetMarker_ = Instantiate(Resources.Load("Prefabs/Unit/TargetMarker"), gameObject.transform) as GameObject;
        }
        targetMarker_.GetComponent<TargetMarker>().setTargetUnit(target);
    }

    public void showDamage(float damage)
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/Unit/FloatingDamage"), gameObject.transform) as GameObject;
        obj.GetComponent<FloatingDamage>().setText(damage.ToString());
        obj.transform.localPosition = new Vector3(0, gameObject.GetComponent<Unit>().height_, 0);
    }

    public void showXp(int xp)
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/Unit/FloatingDamage"), gameObject.transform) as GameObject;
        obj.GetComponent<FloatingDamage>().setText(xp.ToString() + " XP");
        obj.GetComponent<FloatingDamage>().setColor(Color.yellow);
        obj.transform.localPosition = new Vector3(0, gameObject.GetComponent<Unit>().height_, 0);
    }

    public void showMessage(string message)
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/Unit/FloatingDamage"), gameObject.transform) as GameObject;
        obj.GetComponent<FloatingDamage>().setText(message);
        obj.GetComponent<FloatingDamage>().setColor(Color.yellow);
        obj.transform.localPosition = new Vector3(0, gameObject.GetComponent<Unit>().height_, 0);
    }
}
