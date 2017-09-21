using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;
using SimpleJSON;

[Serializable]
public class Data {
    private bool type;
    public int Points;
	public int numTotal;
    public float Preference;
    public JSONArray avgStats = new JSONArray();
    public JSONArray statStdDev = new JSONArray();
    public JSONObject Stats = new JSONObject();

    public Data(Level lev){
        this.type = false;
        this.Points = lev.Points;
        this.numTotal = lev.numTotal;
        this.Preference = lev.Preference;
        for(int stat = 0; stat < lev.Stats.Count; stat++) {
            JSONArray tempArray = new JSONArray();
            this.Stats.Add(lev.Stats[stat].Key.ToString(), tempArray);
            for (int four = 0; four < 4; four++){
                tempArray.Add(lev.Stats[stat].Value[four]);
                if(stat == 0){
                    this.avgStats.Add(lev.avgStats[four]);
                    this.statStdDev.Add(lev.stdDevStats[four]);
                }
            }
        }
    }
    public Data(Type type){
        this.type = true;
        this.Points = type.Points;
        this.numTotal = type.getNumTotal();
        this.Preference = type.Preference;
    }

    public JSONObject createJSON(){
        JSONObject tempObject = new JSONObject();
        tempObject.Add("Points", this.Points);
        tempObject.Add("numTotal", this.numTotal);
        tempObject.Add("Preference", this.Preference);
        if (!type){
            tempObject.Add("avgStats", this.avgStats);
            tempObject.Add("statStdDev", this.statStdDev);
            tempObject.Add("Stats", this.Stats);
        }
        return tempObject;
    }
}

public class StatJSON : MonoBehaviour {
    JSONObject dataList = new JSONObject(); // Top level JSON object

    void Start(){
        // Add time/seed information
        dataList.Add("File ID", new JSONObject());
        dataList.Add("Timed Data", new JSONObject());
        dataList.Add("Types Log", new JSONObject());
        dataList["File ID"].Add("Date", System.DateTime.Now.ToLongDateString());
        dataList["File ID"].Add("Time", System.DateTime.Now.ToLongTimeString());
        dataList["File ID"].Add("Seed", MasterController.seed);
        dataList["File ID"].Add("Adaptive", GenerateInfiniteFull.adapt);
    }

	public void LogDeath(){
        // Create the json file that will be used to record all of the data
        string fileName = string.Format("/Logs/DataLog{0}.json", MasterController.seed);

        // Loop through every Level and create the json-ready version of it
        for (int type = 0; type < 4; type++){               // 4 = MasterController.Types.Count
            JSONObject JSONType = new JSONObject();
            Type TYPEType = MasterController.Types[type];
            JSONType.Add("Type Stats", new Data(TYPEType).createJSON());
            for (int lev = 0; lev < 3; lev++){              // 3 = MasterController.Types[type].Levels.Count
                JSONType.Add(lev.ToString(), new Data(MasterController.Types[type].Levels[lev]).createJSON());
            }
            dataList["Types Log"].Add(TYPEType.name, JSONType);
        }

        // Print out the dataList json to a unique file
        string json = dataList.ToString();
        File.WriteAllText(Application.dataPath + fileName, json);
	}

    public void ThirtySecondLog(){
        int numObjTouched = 0;
        JSONObject recap = new JSONObject();

        // Add current values to JSON object and store with the current time as the key
        recap.Add("Preferences", new JSONObject());
        
        recap.Add("NumObjTouched", new JSONObject());
        recap.Add("TotalPoints", MasterController.totalPoints);
        recap.Add("Orbs", MasterController.orbs);
        recap.Add("Intro", GenerateInfiniteFull.intro);
        recap.Add("Deaths", MasterController.deaths);

        recap.Add("Jumps", MasterController.jumps);
        recap.Add("TimesReset", MasterController.timesReset);
        recap.Add("AvgSpeed", MasterController.AvgSpeed);
        
        recap.Add("Tricks", new JSONObject());
        recap["Tricks"].Add("Flips", MasterController.totalFlips);
        recap["Tricks"].Add("Spins", MasterController.totalSpins);
        recap["Tricks"].Add("Turns", MasterController.totalTurns);
        recap["Tricks"].Add("Perfect Flips", MasterController.perfectFlips);
        recap["Tricks"].Add("Perfect Spins", MasterController.perfectSpins);
        recap["Tricks"].Add("Perfect Turns", MasterController.perfectTurns);
        
        recap.Add("Frames", new JSONObject());
        recap["Frames"].Add("framesAtMax", MasterController.framesAtMax);
        recap["Frames"].Add("framesBoosting", MasterController.framesBoosting);
        recap["Frames"].Add("framesDrifting", MasterController.framesDrifting);
        recap["Frames"].Add("framesInAir", MasterController.framesInAir);
        recap["Frames"].Add("framesOnBack", MasterController.framesOnBack);

        for (int type = 0; type < 4; type++){
            JSONObject JSONType = new JSONObject();
            Type TYPEType = MasterController.Types[type];
            numObjTouched += TYPEType.getNumTotal();
            recap["Preferences"].Add(TYPEType.name, JSONType);
            for (int level = 0; level < 3; level++){
                int pref = TYPEType.Levels[level].Preference;
                JSONType.Add(string.Format("Level: {0}", level), pref);
            }
        }

        recap["numObjTouched"] = numObjTouched;
        dataList["Timed Data"].Add(Time.timeSinceLevelLoad.ToString(), recap);
        
        // Debugging
        //print(Time.timeSinceLevelLoad);
    }
}