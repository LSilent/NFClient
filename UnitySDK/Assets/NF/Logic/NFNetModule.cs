﻿using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Google.Protobuf;

namespace NFSDK
{
	public class NFNetModule : NFIModule
    {
		private NFHelpModule mHelpModule;



        public override void Awake()
        {
        }

        public override void Init()
        {
        }

        public override void Execute()
        {
			NFCNet.Instance().Execute();
        }

        public override void BeforeShut()
        {
        }

        public override void Shut()
        {
        }


        public NFNetModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
		}

        public override void AfterInit()
        {
			mHelpModule = mPluginManager.FindModule<NFHelpModule>();

        }

         
        public void ConnectServer(string ip, UInt16 port)
        {
            if (NFCNet.Instance().isConnected())
                NFCNet.Instance().shutDown();

            if (ip == "127.0.0.1" && port != 14001)
            {
                ip = NFCNet.Instance().ip;
            }

            NFCNet.Instance().ready(ip, port);
            NFCNet.Instance().connect();
        }

        public void AddReceiveCallBack(NFMsg.EGameMsgID id, NFCMessageDispatcher.MessageHandler netHandler)
        {
            NFCNetDispatcher.Instance().AddReceiveCallBack((UInt16)id, netHandler);
        }

        public void SendToServerByPB(NFMsg.EGameMsgID unMsgID, MemoryStream stream)
        {
            NFMsg.MsgBase xData = new NFMsg.MsgBase();
			xData.player_id = mHelpModule.NFToPB(mOwnerID);
            xData.msg_data = ByteString.AttachBytes(stream.ToArray());
            
            MemoryStream body = new MemoryStream();
            xData.WriteTo(body);

            MemoryStream pack = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(pack);
            UInt32 msgLen = (UInt32)body.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;
            writer.Write(NFCNet.ConvertUint16((UInt16)unMsgID));
            writer.Write(NFCNet.ConvertUint32((UInt32)msgLen));
            body.WriteTo(pack);

            NFCNet.Instance().sendMsg(pack);
        }

        public NFGUID mOwnerID = new NFGUID();
    }
}