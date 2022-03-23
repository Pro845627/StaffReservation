using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using HarmonyLib;
using LiteNetLib.Utils;
using MEC;
using Mirror.LiteNetLib4Mirror;
using NorthwoodLib;
using NorthwoodLib.Pools;

namespace StaffReservation
{
    public class PluginConfig : IConfig
    {
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;
        [Description("The max slots of reservation")]
        public int MaxSlots { get; set; } = 1;
        [Description("List of groups who will be in reservation")]
        public List<string> GroupsList { get; set; } = new List<string>()
        {
            "owner", "admin", "moderator"
        };
        [Description("List of players who will be in reservation. If the sepcified player is a member of any group above, you don't need to add him")]
        public List<string> UserIdList { get; set; } = new List<string>
        {
            "SomeOnesUserId@steam"
        };
    }
    public class SRPlugin : Plugin<PluginConfig>
    {
        public override string Author => "Gsuto_Maple";
        public override string Name => "Staff Reservation";
        public override Version RequiredExiledVersion => Version.Parse("5.0.0");
        public Harmony Harmony { get; set; }
        public static SRPlugin singleton;
        public int Reservations => Player.List.Where(p => HasReservation(p.UserId)).Count();
        public override void OnEnabled()
        {
            singleton = this;
            base.OnEnabled();
            Harmony = new Harmony("com.gsutomaple.staffreservation");
            Harmony.PatchAll();
            Exiled.Events.Handlers.Player.PreAuthenticating += Player_PreAuthenticating;
		}
        public override void OnDisabled()
        {
            base.OnDisabled();
            Harmony.UnpatchAll();
            Exiled.Events.Handlers.Player.PreAuthenticating -= Player_PreAuthenticating;
        }
        private void Player_PreAuthenticating(Exiled.Events.EventArgs.PreAuthenticatingEventArgs ev)
        {
            int max = singleton.Config.MaxSlots;
            if (Reservations < max && HasReservation(ev.UserId)) //预留空位，有预留
                return; 
            //预留无空位或无预留
            int normal = Player.List.Count() - (Reservations >= max ? max : Reservations);
            int normal_max = CustomNetworkManager.slots - max;
            if (normal >= normal_max) //正常位满，无预留
            {
                NetDataWriter netDataWriter = new NetDataWriter();
                netDataWriter.Reset();
                netDataWriter.Put((byte)1);
                ev.Request.Reject(netDataWriter);
                ev.Disallow();
            }
        }
        public static bool HasReservation(string userid)
        {
            if (singleton.Config.UserIdList.Contains(userid))
                return true;
            if (ServerStatic.SharedGroupsMembersConfig.GetStringDictionary("SharedMembers").TryGetValue(userid, out string group))
            {
                if (singleton.Config.GroupsList.Contains(group))
                    return true;
            }
            return false;
        }
	}
	[HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RefreshServerData))]
	public static class PublicListPatch
    {
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> insertion = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(SRPlugin), nameof(SRPlugin.singleton))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Plugin<PluginConfig>), nameof(Plugin<PluginConfig>.Config))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PluginConfig), nameof(PluginConfig.MaxSlots))),
                new CodeInstruction(OpCodes.Sub, null)
            };
            var a = instructions.ToList();
            a.InsertRange(161, insertion);
            a.InsertRange(224 + insertion.Count, insertion);
            return a;
        }
    }
}
