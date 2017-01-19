using log4net;
using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using MiNET.Blocks;
using MiNET.Entities;
using MiNET.Entities.Projectiles;
using MiNET.Utils;
using MiNET.Worlds;

namespace JetPack_MiNET
{

    [Plugin(PluginName = "JetPack_MiNET", Description = "JetPack for MiNET", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : Plugin
    {
        Dictionary<string, Boolean> gun = new Dictionary<string, Boolean>();
        Dictionary<string, Boolean> hook = new Dictionary<string, Boolean>();

        protected static ILog _log = LogManager.GetLogger("MyAuth");

        protected override void OnEnable()
        {
            _log.Warn("Loaded");
            
        }


        //疑似コマンド関数
        [PacketHandler]
        public void OnChat(McpeInteract packet, Player player)
        {
            var item = player.Inventory.GetItemInHand().Id;
            if (item == 1)
            {
                Vector3 MotionJ = new Vector3(0, 0, 0);
                MotionJ.Y = 0.8F;
                /*
                 * 
                 * 
                 * setMotion->updateMovement()->
                 * 
                 * addMovement->addPlayerMovement-＞addPlayerMovement
                 * or
                 * addEntityMotion
                 * //addEntityMovementってなんだよ！
                 * 
                 * */

                /*MotionJ
                String jump = getConfig().get("jump").toString();
                MotionJ.y = new Float(jump);
                player.setMotion(MotionJ);*/
            } //watch
        }

        [PacketHandler]
        public Package Hookshot(McpeUseItem packet, Player player)
        {
            if (!(packet.face == -1 & player.Inventory.GetItemInHand().Id == 346)) return packet;
            player.Level.BroadcastMessage("test: arrow", type: MessageType.Raw); // Message typeはtip popup messageが選べる！

            const int force = 1;
            //Arrow arrow = new Arrow(player, player.Level, !(force < 1.0));
            Arrow arrow = new Arrow(player, player.Level);
            arrow.KnownPosition = (PlayerLocation)player.KnownPosition.Clone();
            arrow.KnownPosition.Y += 1.62f;

            arrow.Velocity = arrow.KnownPosition.GetHeadDirection() * (force * 2.0f * 1.5f);
            arrow.KnownPosition.Yaw = (float)arrow.Velocity.GetYaw();
            arrow.KnownPosition.Pitch = (float)arrow.Velocity.GetPitch();
            arrow.BroadcastMovement = true;
            arrow.DespawnOnImpact = false;

            //arrow.HitEvent += testary;

            arrow.SpawnEntity();
            var setEntityLinkPk = McpeSetEntityLink.CreateObject();
            setEntityLinkPk.riderId = 0;
            setEntityLinkPk.riddenId = arrow.EntityId;
            setEntityLinkPk.linkType = 2;
            /*    public static final byte TYPE_REMOVE = 0;
                     public static final byte TYPE_RIDE = 1;
                 public static final byte TYPE_PASSENGER = 2; 
             */
            new Task(() => player.Level.RelayBroadcast(setEntityLinkPk)).Start();




            Task.Run(() =>
            {
                PlayerLocation pos = null;
                while (arrow.Velocity.Length() > 0)
                {
                    pos = arrow.KnownPosition;
                   McpeMovePlayer mp = McpeMovePlayer.CreateObject();
                    mp.entityId = 0;
                    mp.x = pos.X;
                    mp.y = pos.Y;
                    mp.z = pos.Z;
                    mp.pitch = player.KnownPosition.Pitch;
                    mp.headYaw = player.KnownPosition.HeadYaw;
                    mp.yaw = player.KnownPosition.Yaw;
                    new Task(() => player.Level.RelayBroadcast(mp)).Start();
                    Thread.Sleep(100);
                }
                /*Console.Write("done!!" + pos);
                Block block = BlockFactory.GetBlockById((byte)1);
                block.Coordinates = new BlockCoordinates((int)arrow.KnownPosition.X, (int)arrow.KnownPosition.Y - 2, (int)arrow.KnownPosition.Z);
                //arrow.Level.SetBlock(block);

                Block item = new Block(35);
                player.KnownPosition.Y -= 2;*/
            });
            return packet;
        }



    }
}
