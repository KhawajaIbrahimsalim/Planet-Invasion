using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class ReadWriteJSON : MonoBehaviour
{
    public void writeJson()
    {
        List<List<int>> DataToWrite = new List<List<int>>();  // assign data to this variable change type accordingly
        JsonData json = JsonMapper.ToJson(DataToWrite);
        Debug.Log(json);
        string destinationPath = System.IO.Path.Combine(Application.persistentDataPath, 0.ToString()); // file name in place of 0
        Debug.Log(destinationPath);
        System.IO.File.WriteAllText(destinationPath, json.ToString());
    }



    public List<List<int>> ReadJSON()
    {
        List<List<int>> DataToRead;   // assign data to this variable and change type accordingly
        string destinationPath = System.IO.Path.Combine(Application.persistentDataPath, 0.ToString()); // file name in place of 0
        Debug.Log(destinationPath);
        string json = System.IO.File.ReadAllText(destinationPath);
        DataToRead = JsonMapper.ToObject<List<List<int>>>(json); //change conversion type accordingly
        return DataToRead;
    }
}
