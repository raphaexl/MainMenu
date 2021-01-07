using UnityEngine;
using System.Collections.Generic;

class CloudManager : MonoBehaviour
{
    public ServerLoadedData serverLoadedData;


    public static CloudManager CM;
    #region Data
    [System.Serializable]
    public class ServerLoadedData
    {
        public List<PlayerData> aircraftdata;
        public List<PlayerData> canondata;
        public List<PlayerData> missiledata;
        public List<PlayerData> agmdata;
        public List<PlayerData> colordata;
        public List<PlayerData> flagdata;
        public List<PlayerData> reactordata;
        public List<PlayerData> wingdata;
        public List<PlayerData> armordata;
        public bool noads;
        public int mtxsum;
        [HideInInspector]
        public int isoffer;
        [HideInInspector]
        public PlayerStatistics playerstatistics;

    }
    #endregion
    [System.Serializable]
    public class PlayerStatistics
    {
        public string playername;
        public int playerxp;
        public int playerlevel;
        public int playercoin;
        public int playerruby;
        public int playerkills;
        public int playerwins;
    }
    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public string ItemId;
        public bool unlocked;
        public uint price;
        public string type;
    }

    /// <summary>
    /// Made public
    /// </summary>
    private void Awake()
    {
        CM = this;

        serverLoadedData = new ServerLoadedData();
        serverLoadedData.aircraftdata = new List<PlayerData>();
        serverLoadedData.canondata = new List<PlayerData>();
        serverLoadedData.missiledata = new List<PlayerData>();
        serverLoadedData.agmdata = new List<PlayerData>();
        serverLoadedData.colordata = new List<PlayerData>();
        serverLoadedData.flagdata = new List<PlayerData>();
        serverLoadedData.wingdata = new List<PlayerData>();
        serverLoadedData.reactordata = new List<PlayerData>();
        serverLoadedData.armordata = new List<PlayerData>();
        RemoveThisAssign(serverLoadedData.aircraftdata);
        RemoveThisAssign(serverLoadedData.canondata);
        RemoveThisAssign(serverLoadedData.missiledata);
        RemoveThisAssign(serverLoadedData.agmdata);
        RemoveThisAssign(serverLoadedData.armordata);
        RemoveThisAssign(serverLoadedData.colordata);
        RemoveThisAssign(serverLoadedData.flagdata);
        RemoveThisAssign(serverLoadedData.reactordata);
        RemoveThisAssign(serverLoadedData.wingdata);
    }

    void RemoveThisAssign(List<CloudManager.PlayerData> data)
    {
        for (int i = 0; i < 5; i++)
        {
            PlayerData temp = new PlayerData();
            temp.ItemId = i.ToString();
            temp.unlocked = (i % 2 == 0) ? true : false;
            temp.name = Random.Range(0, 100).ToString();
            data.Add(temp);
        }
    }


}