using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MBehavior {

	static NetworkManager s_Instance;
	public static NetworkManager Instance{ get { return s_Instance; }}
	public NetworkManager(){ s_Instance = this; }

	NetworkClient client;
	int port = 7777;

	public enum MyMsgType
	{
		ClientSetupID = 100,
		PlaceHeroSend = 101,
		PlaceHeroRecieve = 102,
		MoveHeroSend = 111,
		MoveHeroRecieve = 112,
	}

	string inputHost = "localhost";
	List<int> clients = new List<int>();
	List<PlaceHeroMessage> placeHeroMsgList = new List<PlaceHeroMessage>();
	List<MoveHeroMessage> moveHeroMsgList = new List<MoveHeroMessage>();

	#region Server
	void InitializeServer()
	{
		NetworkServer.Listen(port);
		NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
		NetworkServer.RegisterHandler((short)MyMsgType.PlaceHeroSend, ServerPlaceHero);
		NetworkServer.RegisterHandler((short)MyMsgType.MoveHeroSend, ServerMoveHero);
	}

	void OnServerConnect(NetworkMessage netMsg)
	{
		Debug.Log(string.Format("Client has connected to server with connection id: {0}", netMsg.conn.connectionId));
		NetworkServer.SetClientReady(netMsg.conn);
		SetIDMessage msg = new SetIDMessage();
		msg.id = netMsg.conn.connectionId;
		clients.Add( msg.id );
		NetworkServer.SendToClient( netMsg.conn.connectionId, (short)MyMsgType.ClientSetupID ,  msg);
	}

	void ServerPlaceHero( NetworkMessage netMsg )
	{
		PlaceHeroMessage msg = netMsg.ReadMessage<PlaceHeroMessage>();

		placeHeroMsgList.Add( msg );

		if ( placeHeroMsgList.Count >= clients.Count )
		{
			foreach( PlaceHeroMessage phMsg in placeHeroMsgList ) {
				foreach( int clientID in clients ) {
					// TODO : CHANGE TO != 
					if ( clientID != phMsg.senderID ) {
//					#region test
//					if ( clientID == phMsg.senderID ) {
//						msg.senderID = 2;
//						foreach( RawHeroInfo h in phMsg.heroInfo ) {
//							h.ID += 1000;
//						}
//					#endregion
						Debug.Log("Send MSg from " + phMsg.senderID + " to " + clientID );
						NetworkServer.SendToClient( clientID , (short)MyMsgType.PlaceHeroRecieve , phMsg );
					}
				}
			}
		}
	}

	void ServerMoveHero( NetworkMessage netMsg )
	{
		Debug.Log("On Server Move Hero");
		MoveHeroMessage msg = netMsg.ReadMessage<MoveHeroMessage>();

		moveHeroMsgList.Add( msg );

		if ( moveHeroMsgList.Count >= clients.Count )
		{
			foreach( MoveHeroMessage mMsg in moveHeroMsgList ) {
				foreach( int clientID in clients ) {
					// TODO : CHANGE TO != 
					if ( clientID != mMsg.senderID ) {
//					#region test
//					if ( clientID == mMsg.senderID ) {
//						msg.senderID = 2;
//						foreach( HeroMoveInfo h in mMsg.heroMoves ) {
//							h.ID += 1000;
//						}
//					#endregion
						NetworkServer.SendToClient( clientID , (short)MyMsgType.MoveHeroRecieve , mMsg );
					}
				}
			}
		}
	}

	#endregion

	#region Client
	private int clientID = -1;
	public static int ClientID{
		get {
			return Instance.clientID;
		}
	}
	void InitializeClient()
	{
		client = new NetworkClient();
		client.RegisterHandler(MsgType.Connect, OnClientConnect);
		client.RegisterHandler( (short)MyMsgType.ClientSetupID, OnSetUpID);
		client.RegisterHandler( (short)MyMsgType.PlaceHeroRecieve, RecievePlaceHero);
		client.RegisterHandler( (short)MyMsgType.MoveHeroRecieve, RecieveMoveHero);
		client.Connect(inputHost, port);
	}

	public void OnSetUpID(NetworkMessage netMsg)
	{
		SetIDMessage msg = netMsg.ReadMessage<SetIDMessage>();
		if ( clientID < 0 )
			clientID = msg.id;
	}

	void OnClientConnect(NetworkMessage netMsg)
	{

	}

	public void SendPlaceHero( RawHeroInfo[] heros )
	{
		PlaceHeroMessage msg = new PlaceHeroMessage();
		msg.senderID = clientID;
		msg.heroInfo = new RawHeroInfo[heros.Length];
		for( int i = 0 ; i < msg.heroInfo.Length ; ++ i )
			msg.heroInfo[i] = heros[i].Copy();
		if ( client != null )
			client.Send( (short) MyMsgType.PlaceHeroSend , msg );
	}

	public void RecievePlaceHero( NetworkMessage netMsg )
	{
		PlaceHeroMessage msg = netMsg.ReadMessage<PlaceHeroMessage>();
		LogicArg arg = new LogicArg( this );
		arg.AddMessage( "msg" , msg );
		M_Event.FireLogicEvent(LogicEvents.NetPlaceHero , arg );

	}

	public void SendMoveHero( HeroMoveInfo[] heros )
	{
		MoveHeroMessage msg = new MoveHeroMessage();
		msg.senderID = clientID;
		msg.heroMoves = new HeroMoveInfo[heros.Length];
		for( int i = 0 ; i < msg.heroMoves.Length ; ++ i )
			msg.heroMoves[i] = heros[i].Copy();
		if ( client != null )
			client.Send( (short) MyMsgType.MoveHeroSend , msg );
		Debug.Log("Send Move Hero Network " + heros.Length );
	}

	public void RecieveMoveHero( NetworkMessage netMsg )
	{
		MoveHeroMessage msg = netMsg.ReadMessage<MoveHeroMessage>();
		LogicArg arg = new LogicArg( this );
		arg.AddMessage( "msg" , msg );
		M_Event.FireLogicEvent(LogicEvents.NetMoveHero , arg );

	}

	#endregion

	void OnGUI()
	{
		if ( client == null )
		{	
			GUILayout.Label("IP:" + Network.player.ipAddress);
			if (GUILayout.Button("Init Server"))
			{
				InitializeServer();
			}
			if (client == null || !client.isConnected)
			{
				inputHost = GUILayout.TextField(inputHost);
				if (GUILayout.Button("Client Connect"))
				{
					InitializeClient();
				}
			}
			
		}else{
			GUILayout.Label("ClientID " + clientID);
		}
	}

}

class SetIDMessage : MessageBase
{
	public int id;
}

class PlaceHeroMessage : MessageBase
{
	public int senderID;
	public RawHeroInfo[] heroInfo;
}

class MoveHeroMessage : MessageBase
{
	public int senderID;
	public HeroMoveInfo[] heroMoves;
}
