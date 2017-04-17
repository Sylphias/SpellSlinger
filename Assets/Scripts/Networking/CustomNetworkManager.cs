using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
    class CustomNetworkManager:NetworkManager
    {
        public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success,extendedInfo,matchInfo);
            if (success)
            {
                NetworkServer.RegisterHandler(MsgType.AddPlayer, OnRequestNonLocalPlayerInit);
            }
        }
        public void OnRequestNonLocalPlayerInit(NetworkMessage msg)
        {
            //NetIDMessage idMsg = msg.ReadMessage<NetIDMessage>();
            //GameObject player = ClientScene.FindLocalObject(idMsg.netId);
            //player.GetComponent<PlayerNetworked>().RpcStartNonOwner();
    }

}

