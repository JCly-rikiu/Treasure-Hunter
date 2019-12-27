using UnityEngine;

public static class Log
{
    static bool devMode = true;

    public static void Status(System.Type type, string message)
    {
        Debug.Log("[" + type.ToString() + "] " + message);
    }

    public static void Dev(System.Type type, string message)
    {
        if (devMode)
        {
            Debug.Log("[" + type.ToString() + "] " + message);
        }
    }
}
