using System.Collections.Generic;
using UnityEngine;

public class PowerUpTimeBased : PowerUp {
    public PowerUpTimeBased(string name, int cost, float interval)
        : base(name, cost) {

        this.interval = interval;
        this.type = Type.TimeBased;

        for (int i = 1; i < level; i++)
            this.interval += interval*0.2f;

        observers = new LinkedList<IObserverPowerUp>();
    }

    public bool EndTrigger {
        get {
            return endTrigger;
        }

        set {
            endTrigger = value;
        }
    }

    public float TimeElasped {
        get {
            return Time.timeSinceLevelLoad - startTime;
        }
    }

    public float TimeRemaining {
        get {
            return interval - TimeElasped;
        }
    }

    public static float CardBonus {
        set {
            cardBonus = value;
        }
    }

    public bool IsEnd() {
        if (!endTrigger && !IsActive()) {
            endTrigger = true;
            return true;
        }
        return false;
    }

    public bool IsActive() {
        return TimeElasped < interval * cardBonus;
    }

    public void Activate() {
        startTime = Time.timeSinceLevelLoad;
        endTrigger = false;
    }

    public override void Upgrade() {
        level++;
        interval *= 1.2f;
        PlayerPrefs.SetInt(name , level);
    }

    public void Subscibe(IObserverPowerUp observer) {
        observers.AddFirst(observer);
    }

    public void activateEndTrigger() {
        endTrigger = true;
        foreach (var observer in observers) {
            observer.OnInvBegin();
        }
    }

    public void Reset() {
        startTime = -10.0f;
        endTrigger = true;
    }


    float interval;
    float startTime = -10.0f;
    bool endTrigger = true;
    static float cardBonus = 1.0f;
    LinkedList<IObserverPowerUp> observers;

}