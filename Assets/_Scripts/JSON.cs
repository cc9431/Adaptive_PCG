using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;
//using LitJson;

[Serializable]
public class Data{
    public int Points;
	public int numTotal;
	public int typeID;
    public int levelID;
    public int Preference;
    public float[] avgStats = new float[4];
    public int[] Stats;

}

public static class JsonHelper{

    public static string ToJson<T>(T[] array){
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>{
        public T[] Items;
    }
}

public class JSON : MonoBehaviour {
    Data[] dataList = new Data[12];
    int count = 0;
    Data data;
    public void StoreData(Level lev, int topID){
        // Create a new data holder and put all of the current level's info into it
        data = new Data();
        data.Points = lev.Points;
        data.numTotal = lev.numTotal;
        data.avgStats = lev.avgStats;
        data.Preference = lev.Preference;
        data.levelID = lev.ID;
        data.typeID = topID;

        // Because the JsonUtility cannot handle List<KeyValuePair<float, int>> we need to turn it into an array of strings
        List<int> tempStats = new List<int>();
        for(int stat = 0; stat < lev.Stats.Count; stat++) {
            tempStats.Add((int)lev.Stats[stat].Key);
            tempStats.Add(lev.Stats[stat].Value[0]);
            tempStats.Add(lev.Stats[stat].Value[1]);
            tempStats.Add(lev.Stats[stat].Value[2]);
            tempStats.Add(lev.Stats[stat].Value[3]);
        }
        data.Stats = tempStats.ToArray();
        
        // Add the level to our array of levels
        dataList[count] = data;
        count++;

        // For Debugging
		// string s = JsonUtility.ToJson(data);
        // Debug.Log(s);
    }

	public void LogDeath(){
        // Create the json file that will be used to record all of the data
        string fileName = string.Format("Logs/DataLog{0}.json", MasterController.seed);

        // Loop through every Level and create the json-ready version of it
        for (int type = 0; type < 4; type++){               // 4 = MasterController.Types.Count
            for (int lev = 0; lev < 3; lev++){              // 3 = MasterController.Types[type].Levels.Count
                StoreData(MasterController.Types[type].Levels[lev], MasterController.Types[type].ID);
            }
        }

        // Print out the dataList json to a unique file
        File.WriteAllText(Application.dataPath + fileName, JsonHelper.ToJson(dataList));
	}
}