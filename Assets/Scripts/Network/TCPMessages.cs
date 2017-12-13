using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPMessages {

    public enum MessageType : int {
        Init, Create, Update, Delete
    }

	public class MainMessage {

        public MessageType type;

    }

    public class CameraMovement {

    }
}
