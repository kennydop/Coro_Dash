using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
public class Billing : MonoBehaviour
{
    string pts_18000 = "com.trayl.coronadash.18000pts";
    string pts_38000 = "com.trayl.coronadash.38000pts";
    string removeads = "com.trayl.coronadash.removeads";
    public  void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == pts_18000)
        {
            GameManager.Instance.TotalPoints += 18000;
            ES3.Save<int>("TotalPoints", GameManager.Instance.TotalPoints);
            Toast.Instance.Show("18000 Points Purchased");
        }   

        if (product.definition.id == pts_38000)
        {
            GameManager.Instance.TotalPoints += 38000;
            ES3.Save<int>("TotalPoints", GameManager.Instance.TotalPoints);
            Toast.Instance.Show("38000 Points Purchased");
        }

        if (product.definition.id == removeads)
        {
            GameManager.Instance.removedAds = true;
            GameManager.Instance.RemovingAds();
            ES3.Save<bool>("Ads Removed", GameManager.Instance.removedAds);
            Toast.Instance.Show("Ads Removed!");
        }
        // GameManager.Instance.TotalPoints += int.Parse(product.Title);
        // ES3.Save<int>("TotalPoints", GameManager.Instance.TotalPoints);
        // Toast.Instance.Show(product.title + "Points Purchased");
    }
// Add restore purchases on iphone
    public  void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Toast.Instance.Show(reason.ToString());
    }
}