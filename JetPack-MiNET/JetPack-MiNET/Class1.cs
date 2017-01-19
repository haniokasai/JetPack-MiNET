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
            setEntityLinkPk.riderId = arrow.EntityId;
            setEntityLinkPk.riddenId =0;
            setEntityLinkPk.linkType = 1;
            /*    public static final byte TYPE_REMOVE = 0;
                     public static final byte TYPE_RIDE = 1;
                 public static final byte TYPE_PASSENGER = 2; 
             */

            /*new Task(() => {
                if (player != null) player.Level.RelayBroadcast(setEntityLinkPk);
            }).Start();*/

            /*var setmotion = new McpeSetEntityMotion();
            setmotion.entityId = player.EntityId;
            var a = player.KnownPosition.ToVector3();
            a.X = 100;
            setmotion.velocity = a;
            player.SendPackage(setEntityLinkPk);*/

            /*McpeAddEntity pk = new McpeAddEntity();
            pk.entityType = (uint)EntityType.Minecart;
            pk.entityId = EntityManager.EntityIdUndefined;
            pk.x = player.KnownPosition.X;
            pk.y = player.KnownPosition.Y;
            pk.z = player.KnownPosition.Z;
            player.SendPackage(pk);

            setEntityLinkPk.riderId = pk.entityId;
            setEntityLinkPk.riddenId = 0;
            setEntityLinkPk.linkType = 1;*/

            /*    public static final byte TYPE_REMOVE = 0;
                     public static final byte TYPE_RIDE = 1;
                 public static final byte TYPE_PASSENGER = 2; 
             */
            //player.SendPackage(setEntityLinkPk);
            //player.SendPackage(setEntityLinkPk);
            //player.BroadcastSetEntityData();

            // player.SendPackage(setEntityLinkPk);

            McpeSetEntityMotion motions = McpeSetEntityMotion.CreateObject();
            motions.entityId = 0;
            var a = player.KnownPosition.ToVector3();
            a.X = 100;
            motions.velocity = a;
            new Task(() => player.Level.RelayBroadcast(motions)).Start();

            McpeMoveEntity moveEntity = McpeMoveEntity.CreateObject();
            moveEntity.entityId = 0;
            var b = player.KnownPosition;
            b.Y = 100;
            moveEntity.position =b;
            new Task(() => player.Level.RelayBroadcast(moveEntity)).Start();

            Task.Run(() =>
            {
                /*PlayerLocation pos = null;
                while (arrow.Velocity.Length() > 0)
                {
                    pos = arrow.KnownPosition;
                    Console.Write(pos);
                    Thread.Sleep(100);
                }
                //Console.Write("done!!" + pos);
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
