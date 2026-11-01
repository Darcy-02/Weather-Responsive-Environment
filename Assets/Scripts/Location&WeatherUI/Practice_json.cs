using UnityEngine;
using System.IO;


[System.Serializable]


public class InventoryItem
{
    public string name;
    public int quantity;
}


public class Practice_json : MonoBehaviour
{
    private void Start()
    {
        // Save
        string json = JsonUtility.ToJson(new InventoryItem { name = "Potion", quantity = 10 });
        File.WriteAllText("Assets/ScriptableObjects/Practice.json", json);


        // Load
        string read = File.ReadAllText("Assets/ScriptableObjects/Practice.json");
        InventoryItem loaded = JsonUtility.FromJson<InventoryItem>(read);
        Debug.Log($"Loaded Item: {loaded.name}, Quantity: {loaded.quantity}");
    }
}