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
    public string background;
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
