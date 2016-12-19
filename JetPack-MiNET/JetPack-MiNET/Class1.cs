using log4net;
using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace JetPack_MiNET
{

    [Plugin(PluginName = "JetPack_MiNET", Description = "JetPack for MiNET", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : IPlugin
    {
        Dictionary<string, Boolean> gun = new Dictionary<string, Boolean>();
        Dictionary<string, Boolean> hook = new Dictionary<string, Boolean>();

        protected static ILog _log = LogManager.GetLogger("MyAuth");

        public void OnEnable(PluginContext context)
        {
            _log.Warn("Loaded");
            
        }

        public void OnDisable()
        {
            _log.Warn("Bye");
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


    }
}
