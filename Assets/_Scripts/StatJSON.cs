using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;
using SimpleJSON;

[Serializable]
public class Data {
    public string type;
    public int Points;
	public int numTotal;
    public float Preference;
    public int ID;
    public float[] avgStats = new float[4];
    public float[] statStdDev = new float[4];
    public List<KeyValuePair<float, int[]>> Stats = new List<KeyValuePair<float, int[]>>();

    public Data(Level lev, string name){
        this.type = name;
        this.Points = lev.Points;
        this.numTotal = lev.numTotal;
        this.Preference = lev.Preference;
        this.ID = lev.ID;
        this.avgStats = lev.avgStats;
        this.statStdDev = lev.stdDevStats;
        this.Stats = lev.Stats;
    }

    public JSONObject LevelsJSON(){
        JSONObject tempObject = new JSONObject();
        tempObject.Add("Type", this.type);
        tempObject.Add("Level", this.ID);
        tempObject.Add("Points", this.Points);
        tempObject.Add("numTotal", this.numTotal);
        tempObject.Add("Preference", this.Preference);

        tempObject.Add("avgAir", this.avgStats[0]);
        tempObject.Add("avgTricks", this.avgStats[1]);
        tempObject.Add("avgSpeed", this.avgStats[2]);
        tempObject.Add("avgPoints", this.avgStats[3]);
        
        tempObject.Add("StdDevAir", this.avgStats[0]);
        tempObject.Add("StdDevTricks", this.avgStats[1]);
        tempObject.Add("StdDevSpeed", this.avgStats[2]);
        tempObject.Add("StdDevPoints", this.avgStats[3]);

        return tempObject;
    }

    public JSONObject StatsJSON(int ObjInter){
        JSONObject tempObject = new JSONObject();

        tempObject.Add("ID", string.Format("{0}{1}", this.type, this.ID));
        tempObject.Add("Time", this.Stats[ObjInter].Key);
        tempObject.Add("Air", this.Stats[ObjInter].Value[0]);
        tempObject.Add("Tricks", this.Stats[ObjInter].Value[1]);
        tempObject.Add("Speed", this.Stats[ObjInter].Value[2]);
        tempObject.Add("Points", this.Stats[ObjInter].Value[3]);

        return tempObject;
    }
}

public class StatJSON : MonoBehaviour {
    JSONObject dataList = new JSONObject(); // Top level JSON object
    float lastLogTime;

    void Start(){
        // Add time/seed information
        dataList.Add("File_ID", new JSONObject());
        dataList.Add("Timed_Data", new JSONArray());
        dataList.Add("Final_Data", new JSONArray());
        dataList.Add("Object_Interactions", new JSONArray());

        // File ID information
        dataList["File_ID"].Add("Date", System.DateTime.Now.ToShortDateString());
        dataList["File_ID"].Add("Time", System.DateTime.Now.ToShortTimeString());
        dataList["File_ID"].Add("Seed", MasterController.seed);
        dataList["File_ID"].Add("Adaptive", GenerateInfiniteFull.adapt);
    }

    void Update(){
        if ((Time.timeSinceLevelLoad - lastLogTime) >= 30){
			lastLogTime = Time.timeSinceLevelLoad;
			ThirtySecondLog();
		}
    }

	public void DataDump(){
        // Loop through every Level and create the json-ready version of it
        for (int type = 0; type < 4; type++){ // 4 = MasterController.Types.Count
            Type TYPEType = MasterController.Types[type];
            for (int lev = 0; lev < 3; lev++){ // 3 = MasterController.Types[type].Levels.Count
                Data datum = new Data(TYPEType.Levels[lev], TYPEType.name);
                JSONObject Level = datum.LevelsJSON();
                dataList["Final_Data"].Add(Level);
                for (int ObjInter = 0; ObjInter < datum.Stats.Count; ObjInter++){
                    JSONObject interaction = datum.StatsJSON(ObjInter);
                    dataList["Object_Interactions"].Add(interaction);   
                }
            }
        }

        // Create the json file that will be used to record all of the data
        string fileName = string.Format("/Logs/{0}", MasterController.seed);
        string fileNameID = fileName + "/ID.csv";
        string fileNameTimed = fileName + "/TimeData.csv";
        string fileNameDump = fileName + "/FinalData.csv";
        string fileNameStats = fileName + "/Stats.csv";

        // Print out the dataList json to a unique file
        string jsonID = JSONObjecttoCSV(dataList["File_ID"].AsObject);
        string jsonTimed = JSONArraytoCSV(dataList["Timed_Data"].AsArray);
        string jsonTypes = JSONArraytoCSV(dataList["Final_Data"].AsArray);
        string jsonStats = JSONArraytoCSV(dataList["Object_Interactions"].AsArray);

        Directory.CreateDirectory(Application.dataPath + fileName);

        File.WriteAllText(Application.dataPath + fileNameID, jsonID);
        File.WriteAllText(Application.dataPath + fileNameTimed, jsonTimed);
        File.WriteAllText(Application.dataPath + fileNameDump, jsonTypes);
        File.WriteAllText(Application.dataPath + fileNameStats, jsonStats);
	}

    public void ThirtySecondLog(){
        int numObjTouched = 0;
        JSONObject recap = new JSONObject();
        recap.Add("Time", Time.timeSinceLevelLoad.ToString());

        // Add current values to JSON object and store with the current time as the key
        recap.Add("Preferences:", "");

        for (int type = 0; type < 4; type++){
            Type TYPEType = MasterController.Types[type];
            numObjTouched += TYPEType.getNumTotal();
            recap.Add(TYPEType.name, TYPEType.Preference);
            for (int level = 0; level < 3; level++){
                int pref = TYPEType.Levels[level].Preference;
                recap.Add(string.Format("{0}L{1}", TYPEType.name, level), pref);
            }
        }
        
        recap.Add("NumObjTouched", numObjTouched);
        recap.Add("TotalPoints", MasterController.totalPoints);
        recap.Add("Orbs", MasterController.orbs);
        recap.Add("Intro", GenerateInfiniteFull.intro);
        recap.Add("Deaths", MasterController.deaths);

        recap.Add("Jumps", MasterController.jumps);
        recap.Add("TimesReset", MasterController.timesReset);
        recap.Add("AvgSpeed", System.Math.Round(MasterController.AvgSpeed, 5));
        
        recap.Add("Tricks:", "");
        recap.Add("Flips", MasterController.totalFlips);
        recap.Add("Spins", MasterController.totalSpins);
        recap.Add("Turns", MasterController.totalTurns);
        recap.Add("Perfect Flips", MasterController.perfectFlips);
        recap.Add("Perfect Turns", MasterController.perfectTurns);
        recap.Add("Perfect Spins", MasterController.perfectSpins);
        
        recap.Add("Frames:", "");
        recap.Add("framesAtMax", MasterController.framesAtMax);
        recap.Add("framesBoosting", MasterController.framesBoosting);
        recap.Add("framesDrifting", MasterController.framesDrifting);
        recap.Add("framesInAir", MasterController.framesInAir);
        recap.Add("framesOnBack", MasterController.framesOnBack);

        dataList["Timed_Data"].Add(recap);
    }

    public string JSONObjecttoCSV(JSONObject json){
        string csv = "";

        foreach (KeyValuePair<string, JSONNode> N in json) csv += (N.Key) + ",";
        csv = csv.Trim(',') + "\n";
        foreach (KeyValuePair<string, JSONNode> N in json) csv += (N.Value) + ",";
        
        return csv.Trim().Trim(',');
    }

    public string JSONArraytoCSV(JSONArray json){
        string csv = "";
        
        foreach (KeyValuePair<string, JSONNode> N in json[0].AsObject) csv += (N.Key) + ",";
        csv = csv.Trim(',') + "\n";

        for (int obj = 0; obj < json.Count; obj++){
            foreach (KeyValuePair<string, JSONNode> N in json[obj].AsObject)
                csv += (N.Value) + ",";

            csv = csv.Trim(',') + "\n";
        }

        return csv.Trim().Trim(',');
    }
}