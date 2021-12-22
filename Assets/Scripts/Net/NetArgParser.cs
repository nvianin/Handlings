using System;
using System.Collections;
using UnityEngine;

public struct NetworkCommand
{
    public string command;
    public string[] args;
    public bool error;

    public string log()
    {
        string value = "";
        value += this.command + ";";
        foreach (string s in this.args)
        {
            value += s;
            value += ":";
        }
        value.Remove(value.Length - 1);
        return value;
    }
}

public class NetArgParser : MonoBehaviour
{
    public NetworkCommand parse(string msg)
    {
        if (msg.Contains(";"))
        {
            string[] comps = msg.Split(';');
            string cmd = comps[0];
            string body = comps[1];

            string[] args = body.Split(':');
            NetworkCommand returnVal = new NetworkCommand { command = cmd, args = args, error = false };
            print("Server command: " + returnVal.log());
            return returnVal;
        }
        else
        {
            return new NetworkCommand { command = "input string does not contain parseable command.", args = new string[0], error = true };
        }
    }
}
