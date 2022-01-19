using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample {
	public class Player : BasePlayer {
		public float posx = 0;
		public float posy = 0;
		public int numberPlayer = 0;
	}

	public class Trees
    {
		public bool isDestroyed;
    }
	

	[RoomType("UnityMushrooms")]
	public class GameCode : Game<Player> {
		

		
		Player actualPlayer;
		List<Player> listPlayers = new List<Player>();




		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
			
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player) {

			listPlayers.Add(player);
			player.numberPlayer = listPlayers.Count-1;

			player.Send("InitializePlayer",player.numberPlayer,player.ConnectUserId);

			foreach(Player pl in Players)
            {
                if (pl.ConnectUserId != player.ConnectUserId)
                {
					pl.Send("PlayerJoined",player.numberPlayer,player.posx,player.posy);
                }
            }

	

		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {
			Broadcast("PlayerLeft", player.ConnectUserId);
			
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player player, Message message) {
			switch (message.Type)
			{
				case "OtherPlayer":
					foreach(Player pl in Players)
                    {
                        if (pl.ConnectUserId == message.GetString(0))
                        {
							 actualPlayer = pl;

						}

						
					}

					foreach(Player pl in Players)
                    {
						if (pl.ConnectUserId != message.GetString(0))
						{
							actualPlayer.Send("InstantiateOtherPlayer", pl.numberPlayer, pl.posx, pl.posy);
						}
					}
					break;

				case "IsCutting":
					foreach (Player pl in Players)
                    {
                        if (pl != player)
                        {
							pl.Send("TreeCut",message.GetInt(0));
                        }
                    }
					break;
				case "NewPosition":
					player.posx = message.GetFloat(0);
					player.posy = message.GetFloat(1);
					foreach (Player pl in Players)
					{
						if (pl != player)
						{
							pl.Send("PlayerMoved",player.numberPlayer,message.GetFloat(0),message.GetFloat(1));
						}
					}
					break;
			}
		}
	}
}