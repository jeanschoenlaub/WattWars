using System.Collections;
using UnityEngine;
using System; 
using Firebase.Database;

[Serializable]
public class DataToSave {
    public string userName;
    public int coins; 
    public int highScore;
}

public class SaveManager : MonoBehaviour
{
    public DataToSave dts;
    public string userId;
    public DatabaseReference dbRef;

    private void Awake() {
         dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveSata() {
        String json = JsonUtility.ToJson(dts);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }


    public void LoadData(){
        StartCoroutine(LoadDataEnum());
    }

    private IEnumerator LoadDataEnum(){
        var serverData = dbRef.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        Debug.Log("Data Loaded");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null){
            Debug.Log("Data found");
            dts = JsonUtility.FromJson<DataToSave>(jsonData);
            Debug.Log(dts.coins);
            
        }else{
            Debug.Log("No data found");
        }
    }
}
