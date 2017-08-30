using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimations : CharacterAnimation {

    bool GoState;
    public UnityArmatureComponent ProfileView;
    public UnityArmatureComponent BackView;

    public float DefaultFadeTime;

    public override void Use()
    {
        if (!BackView.gameObject.activeInHierarchy)
        {
            ProfileView.gameObject.SetActive(false);
            BackView.gameObject.SetActive(true);
        }
            BackView.animation.FadeIn("hack", DefaultFadeTime);
    }

    public override void Run()
    {
        if (!ProfileView.gameObject.activeInHierarchy)
        {
            ProfileView.gameObject.SetActive(true);
            BackView.gameObject.SetActive(false);
            ProfileView.animation.FadeIn("run", DefaultFadeTime);
            GoState = true;
            return;
        } else if (ProfileView.animation.lastAnimationName == "jump_end" && !ProfileView.animation.isCompleted)
            return;
        if (!GoState)
        {
            ProfileView.animation.FadeIn("run", DefaultFadeTime);
            ProfileView.animation.timeScale = 1;
            GoState = true;
        }
    }

    public override void Idle()
    {
        if (!ProfileView.gameObject.activeInHierarchy)
        {
            ProfileView.gameObject.SetActive(true);
            BackView.gameObject.SetActive(false);
            ProfileView.animation.FadeIn("pasiv", DefaultFadeTime);
            ProfileView.animation.timeScale = 1;
            GoState = false;
            return;
        } else if (ProfileView.animation.lastAnimationName == "jump_end" && !ProfileView.animation.isCompleted)
            return;
        if (GoState)
        {
            ProfileView.animation.FadeIn("pasiv", DefaultFadeTime);
            ProfileView.animation.timeScale = 1;
            GoState = false;
        }
    }

    public override void IdleAtWall()
    {
        if (GoState)
        {
            ProfileView.animation.FadeIn("wall_pasiv", DefaultFadeTime);
            ProfileView.animation.timeScale = 1;
            GoState = false;
        }
    }

    public override void Jump()
    {
        //ProfileView.animation.FadeIn("jump_begin", 0.2f, 1);
        //ProfileView.AddEventListener(EventObject.COMPLETE, JumpCompleteListener);
    }

    public override void Fall()
    {
        if (!ProfileView.gameObject.activeInHierarchy)
        {
            ProfileView.gameObject.SetActive(true);
            BackView.gameObject.SetActive(false);
        }
        if (ProfileView.animation.lastAnimationName == "jump_begin" && !ProfileView.animation.isCompleted)
            return;
        ProfileView.animation.FadeIn("flight", DefaultFadeTime);
        ProfileView.animation.timeScale = 1;
    }

    public override void TouchDown()
    {
        if (GoState)
            ProfileView.animation.FadeIn("run", DefaultFadeTime);
        else
            ProfileView.animation.FadeIn("pasiv", DefaultFadeTime);
        ProfileView.animation.timeScale = 1;
    }

    public override void Hanging()
    {
        ProfileView.animation.FadeIn("hanging", DefaultFadeTime);
        ProfileView.animation.timeScale = 1;
    }

    public override void Climb()
    {
        ProfileView.animation.FadeIn("climbs", DefaultFadeTime, 1);
        ProfileView.animation.timeScale = 1;
    }

    public override void LadderGoUp()
    {
        if (BackView.animation.lastAnimationName != "climb_the_stairs")
            BackView.animation.FadeIn("climb_the_stairs", DefaultFadeTime);
        BackView.animation.timeScale = -1;
    }

    public override void LadderGoDown()
    {
        if (BackView.animation.lastAnimationName != "climb_the_stairs")
            BackView.animation.FadeIn("climb_the_stairs", DefaultFadeTime);
        BackView.animation.timeScale = 1;
    }

    public override void LadderIdle()
    {
        if (BackView.animation.lastAnimationName != "climb_the_stairs")
            BackView.animation.FadeIn("climb_the_stairs", DefaultFadeTime);
        BackView.animation.timeScale = 0;
    }

    public override void CrawlGo()
    {
        if (ProfileView.animation.lastAnimationName == "squat" && !ProfileView.animation.isCompleted)
            return;
        if (!GoState)
        {
            ProfileView.animation.FadeIn("crawl", DefaultFadeTime);
            ProfileView.animation.timeScale = 1;
            GoState = true;
        }
    }

    public override void CrawlIdle()
    {
        if (ProfileView.animation.lastAnimationName == "squat" && !ProfileView.animation.isCompleted)
            return;
        if (GoState)
        {
            if (ProfileView.animation.lastAnimationName != "crawl")
                ProfileView.animation.FadeIn("crawl", DefaultFadeTime);
            ProfileView.animation.timeScale = 0;
            GoState = false;
        }
    }

    public override void StartCrawl()
    {
        if (GoState)
            ProfileView.animation.FadeIn("crawl", DefaultFadeTime * 2);
        else
            ProfileView.animation.FadeIn("crawl", DefaultFadeTime * 3);
        /*ProfileView.animation.FadeIn("squat", DefaultFadeTime, 1);
        ProfileView.animation.timeScale = 1;*/
    }

    public override void StopCrawl()
    {
        if (GoState)
            ProfileView.animation.FadeIn("run", DefaultFadeTime * 2);
        else
            ProfileView.animation.FadeIn("pasiv", DefaultFadeTime * 3);
        //ProfileView.animation.FadeIn("squat", 1f, 1);
        //ProfileView.animation.timeScale = -1;
    }

    public override void Defeat()
    {
        throw new NotImplementedException();
    }

    // Use this for initialization
    void Start ()
    {
        ProfileView.animation.Play("pasiv");
    }

    // Update is called once per frame
    void Update()
    {
    }
    
}
