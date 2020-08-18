using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : MonoBehaviour, IStoreListener {
	private static IStoreController m_StoreController; // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
	public static string [] characters = {"character1", "character2", "character3", "character4"};
    public DialogOk dialogOk;
    
    private static string kProductNameAppleSubscription =  "com.unity3d.subscription.new";
    private static string kProductNameGooglePlaySubscription =  "com.unity3d.subscription.original"; 

	// Use this for initialization
	void Start () {
		if (m_StoreController == null) {
			InitializePurchasing();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitializePurchasing() 
    {
        if (IsInitialized()) {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < characters.Length; i++)
            builder.AddProduct(characters[i], ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyCharacter(int x) {
        BuyProductID(characters[x]);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)  {
        if (String.Equals(args.purchasedProduct.definition.id, characters[0], StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            PlayerPrefs.SetInt("Character1", -1);
            PlayerPrefs.SetInt("EnableAds",0);
        } else if (String.Equals(args.purchasedProduct.definition.id, characters[1], StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            PlayerPrefs.SetInt("Character2", -1);
            PlayerPrefs.SetInt("EnableAds",0);
        } else if (String.Equals(args.purchasedProduct.definition.id, characters[2], StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            PlayerPrefs.SetInt("Character3", -1);
            PlayerPrefs.SetInt("EnableAds",0);
        } else if (String.Equals(args.purchasedProduct.definition.id, characters[3], StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            PlayerPrefs.SetInt("Character4", -1);
            PlayerPrefs.SetInt("EnableAds",0);
        } else {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            dialogOk.InvokeDialog("Purchase failed. Unrecognized product");
        }

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        dialogOk.InvokeDialog("Purchase failed. An unknown error has occoured");
    }

    void BuyProductID(string productId) {
        if (!IsInitialized()) {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            dialogOk.InvokeDialog("Failed to initialize purchase");
            return;
        }

        Product product = m_StoreController.products.WithID(productId);

        if (product == null || !product.availableToPurchase) {
            Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            dialogOk.InvokeDialog("BuyProductID: FAIL. Product not found.");
            return;
        }

        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
        m_StoreController.InitiatePurchase(product);
    }

    private bool IsInitialized() {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

}
