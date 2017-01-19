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
           /* var setEntityLinkPk = McpeSetEntityLink.CreateObject();
            setEntityLinkPk.riderId = 0;
            setEntityLinkPk.riddenId = arrow.EntityId;
            setEntityLinkPk.linkType = 2;
            /*    public static final byte TYPE_REMOVE = 0;
                     public static final byte TYPE_RIDE = 1;
                 public static final byte TYPE_PASSENGER = 2; 
             */
            //new Task(() => player.Level.RelayBroadcast(setEntityLinkPk)).Start();





            Task.Run(() =>
            {
                PlayerLocation pos = null;
                while (arrow.Velocity.Length() > 0)
                {
                    pos = arrow.KnownPosition;
                    Thread.Sleep(100);
                }
                var CurrentLocation = player.KnownPosition;
                PlayerLocation lookAtPos = LookAt(CurrentLocation.ToVector3() + new Vector3(0, 1.62f, 0), arrow.KnownPosition.ToVector3());


                // First just rotate towards target pos
                McpeMovePlayer movePlayerPacket = McpeMovePlayer.CreateObject();
                movePlayerPacket.entityId = player.EntityId;
                movePlayerPacket.x = pos.X;
                movePlayerPacket.y = pos.Y;
                movePlayerPacket.z = pos.Z; 
                movePlayerPacket.yaw = lookAtPos.Yaw;
                movePlayerPacket.pitch = lookAtPos.Pitch;
                movePlayerPacket.headYaw = lookAtPos.HeadYaw;
                new Task(() => player.Level.RelayBroadcast(movePlayerPacket)).Start();
                /*Console.Write("done!!" + pos);
                Block block = BlockFactory.GetBlockById((byte)1);
                block.Coordinates = new BlockCoordinates((int)arrow.KnownPosition.X, (int)arrow.KnownPosition.Y - 2, (int)arrow.KnownPosition.Z);
                //arrow.Level.SetBlock(block);

                Block item = new Block(35);
                player.KnownPosition.Y -= 2;*/
            });
            return packet;
        }


        public static PlayerLocation LookAt(Vector3 sourceLocation, Vector3 targetLocation)
        {
            var dx = targetLocation.X - sourceLocation.X;
            var dz = targetLocation.Z - sourceLocation.Z;

            var pos = new PlayerLocation(sourceLocation.X, sourceLocation.Y, sourceLocation.Z);
            if (dx > 0 || dz > 0)
            {
                double tanOutput = 90 - RadianToDegree(Math.Atan(dx / (dz)));
                double thetaOffset = 270d;
                if (dz < 0)
                {
                    thetaOffset = 90;
                }
                var yaw = thetaOffset + tanOutput;

                double bDiff = Math.Sqrt((dx * dx) + (dz * dz));
                var dy = (sourceLocation.Y) - (targetLocation.Y);
                double pitch = RadianToDegree(Math.Atan(dy / (bDiff)));

                pos.Yaw = (float)yaw;
                pos.HeadYaw = (float)yaw;
                pos.Pitch = (float)pitch;
            }

            return pos;
        }

        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }




    }
}
