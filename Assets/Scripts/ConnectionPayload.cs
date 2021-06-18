using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Payload - это то, что мы передаём по сети серверу
// Serializable означает, что класс будет переведёт в массив байтов, потому что
// по сети мы может передавать только массив байтов
[Serializable]
public class ConnectionPayload
{
    public string password;
    public string playerName;


}
