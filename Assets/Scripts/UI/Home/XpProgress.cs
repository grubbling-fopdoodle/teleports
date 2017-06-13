﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class XpProgress : MonoBehaviour {

    public Text text_;
    public Slider slider_;
    public Text levelText_;

    static int xp_, targetXp_;

    //animation
    const float
        animationStartSpeed_ = 0,
        animationAcceleration_ = 2000;

    static bool animationStarted_;
    static float animationCurrentSpeed_;


    void Awake()
    {
    }
    
    void Start () {
        xp_ = GlobalData.instance.playerData_.startXp;
        targetXp_ = GlobalData.instance.playerData_.xp;
        updateUI();
    }
	
	// Update is called once per frame
	void Update () {
        if (animationStarted_)
        {
            float dTime = Time.deltaTime;

            xp_ += (int)(animationCurrentSpeed_ * dTime);

            animationCurrentSpeed_ += animationAcceleration_ * dTime;

            if(xp_ >= targetXp_)
            {
                xp_ = targetXp_;
                stopAnimation();
            }

            updateUI();
        }
        else if(xp_ != targetXp_)
        {
            xp_ = targetXp_;
            updateUI();
        }
    }

    void updateUI()
    {
        int
            currentXp = XpLevels.currentXp(xp_),
            requiredXp = XpLevels.requiredXp(xp_);

        text_.text = currentXp.ToString() + " / " + requiredXp.ToString();
        if (requiredXp != 0)
            slider_.value = (float)currentXp / requiredXp;
        else slider_.value = 0;

        levelText_.text = XpLevels.level(xp_).ToString();
    }

    public static void startAnimation()
    {
        animationStarted_ = true;
        animationCurrentSpeed_ = animationStartSpeed_;
    }

    void stopAnimation()
    {
        animationStarted_ = false;
    }
}
