using System;
using System.Collections.Generic;

public class MainJsonObject {

    public string action;
    public string content;
    public ContentJsonObject contentObj;
    public int msgId;
}

public class ConnectionJsonObject {

    public string connection;
}

public class ContentJsonObject {

    public int id = -1;
    public int type;
    public string typeFig;
    public string color;
    public float coordX;
    public float coordY;
    public float coordZ;
    public float rotX;
    public float rotY;
    public float rotZ;
    public float dimX;
    public float dimY;
    public float dimZ;
}

public class SendMessageJson {
    public string action;
    public int msgId;
    public int objId;
    public string type;
    public ContentJsonObject contentObj;
}
