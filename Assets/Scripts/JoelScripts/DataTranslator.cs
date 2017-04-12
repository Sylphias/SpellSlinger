using System;
using UnityEngine;

public class DataTranslator : MonoBehaviour {
    private static string killString = "[KILLS]";
    private static string deathString = "[DEATHS]";

    public static string ValuesToData(int kills, int deaths)
    {
        return killString + kills + "/" + deathString + deaths;
    }

    private static string DataToValue(string data, string killDeathString)
    {
        string[] dataArrary = data.Split('/');
        foreach (string dataInArray in dataArrary)
        {
            if (dataInArray.StartsWith(killDeathString))
            {
                return dataInArray.Substring(killDeathString.Length);
            }
        }
        Debug.LogError(killDeathString + " cannot be found in " + data);
        return "";
    }

    public static int DataToKills(string data)
    {
        return int.Parse(DataToValue(data, killString));
    }

    public static int DataToDeaths(string data)
    {
        return int.Parse(DataToValue(data, killString));
    }
}
