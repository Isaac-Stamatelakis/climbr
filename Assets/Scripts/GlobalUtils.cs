using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalUtils
{
    public static bool AreKeysInputed(KeyCode[] keycodes)
    {
        foreach (KeyCode keyCode in keycodes)
        {
            if (Input.GetKey(keyCode)) return true;
        }

        return false;
    }
}
