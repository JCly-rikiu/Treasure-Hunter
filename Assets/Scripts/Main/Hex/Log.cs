using UnityEngine;

public static class Log
{
    public static void Status(System.Type type, string message)
    {
        Debug.Log("[" + type.ToString() + "] " + message);
    }
}
