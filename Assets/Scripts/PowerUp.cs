using UnityEngine;

public enum Type {
    TimeBased,
    PieceBased
}
public abstract class PowerUp
{
    public PowerUp(string name, int cost) {
        this.name = name;
        this.level = PlayerPrefs.GetInt(name, 1);
        this.cost = cost;
    }

    public int Level {
        get {
            return level;
        }
    }

    public string Name {
        get {
            return name;
        }
    }

    public int Cost {
        get {
            return cost * level;
        }
    }

    public Type GetType() {
        return type;
    }

    public bool MaxedOut() {
        return level >= 5;
    }

    public abstract void Upgrade();

    protected int level;
    protected Type type;
    protected string name;
    protected int cost;
}