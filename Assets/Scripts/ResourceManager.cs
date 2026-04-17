using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int Gold = 20;
    public int Wood = 10;

    void Awake() => Instance = this;

    public void CollectFromBuildings()
    {
        foreach (var b in FindObjectsByType<Building>(FindObjectsSortMode.None))
        {
            Gold += b.GoldPerTurn;
            Wood += b.WoodPerTurn;
        }
        Debug.Log($"Ресурсы: Золото={Gold} | Дерево={Wood}");
    }

    public bool Spend(int goldCost, int woodCost)
    {
        if (Gold < goldCost || Wood < woodCost)
        {
            Debug.Log($"Недостаточно ресурсов! Нужно: {goldCost} зол. {woodCost} дер.");
            return false;
        }
        Gold -= goldCost;
        Wood -= woodCost;
        return true;
    }
}
