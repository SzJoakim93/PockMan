using UnityEngine;

public class PowerUpPieceBased : PowerUp {
    public PowerUpPieceBased(string name, int cost, int maxQuantity)
        : base(name, cost) {

        this.maxQuantity = maxQuantity;
        this.type = Type.PieceBased;

        for (int i = 1; i < level; i++)
            this.maxQuantity++;
    }

    public override void Upgrade() {
        level++;
        maxQuantity++;
        PlayerPrefs.SetInt(name , level);
    }

    public void Charge() {
        quantity = maxQuantity;
    }

    public int MaxQuantity {
        get {
            return maxQuantity;
        }
    }

    public int Quantity {
        get {
            return quantity;
        }

        set {
            this.quantity = value;
        }
    }

    int quantity;
    int maxQuantity;
}