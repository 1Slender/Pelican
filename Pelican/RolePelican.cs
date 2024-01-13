
using System.Collections.Generic;
using UnityEngine;
using PlayerRoles;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace Pelican
{
    [CustomRole(PlayerRoles.RoleTypeId.Flamingo)]
    public class RolePelican : CustomRole
    {
        public override uint Id { get; set; } = 0;
        public override int MaxHealth { get; set; } = Main.Plugin.Config.MaxHealth;
        public override string Name { get; set; } = "Pelican";
        public override string Description { get; set; } = $"{Main.Plugin.Config.Description}";
        public override string CustomInfo { get; set; } = $"{Main.Plugin.Config.CustomInfo}";
        public override Dictionary<RoleTypeId, float> CustomRoleFFMultiplier { get; set; } = new Dictionary<RoleTypeId, float> { { RoleTypeId.Scp049, 0f }, { RoleTypeId.Scp096, 0f }, { RoleTypeId.Scp106, 0f }, { RoleTypeId.Scp173, 0f }, { RoleTypeId.Scp939, 0f }, { RoleTypeId.Scp3114, 0f } };


        private Dictionary<Player, List<Player>> dictionary = new Dictionary<Player, List<Player>>();

        private Dictionary<Player, RoleTypeId> playerRole = new Dictionary<Player, RoleTypeId>();

        protected override void RoleAdded(Player player)
        {
            Log.Debug("RoleAdded: Пеликан появился");
            if (Main.Plugin.Config.EnableBroadcastRoleAdded) Map.Broadcast(3, Main.Plugin.Config.BroadcastRoleAdded);
            base.RoleAdded(player);
        }

        protected override void SubscribeEvents()
        {
            Log.Debug("Pelican: SubscribeEvents");

            PlayerEvent.Dying += OnDying;
            PlayerEvent.Verified += OnVerified;

            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Log.Debug("Pelican: UnsubscribeEvents");

            PlayerEvent.Dying -= OnDying;
            PlayerEvent.Verified -= OnVerified;

            base.UnsubscribeEvents();
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Player == null || ev.Attacker == null || ev.Player.IsScp) return;

            if (Check(ev.Player))
            {
                Log.Debug($"OnDying: Пеликан {ev.Player.Nickname} погиб");
                Vector3 position = ev.Player.Position;

                if (dictionary.ContainsKey(ev.Player) && dictionary.TryGetValue(ev.Player, out List<Player> deadPlayer))
                {
                    foreach (Player player in deadPlayer)
                    {
                        playerRole.TryGetValue(player, out RoleTypeId roleTypeId);

                        if (player.IsConnected && player.IsDead)
                        {
                            player.Role.Set(roleTypeId, RoleSpawnFlags.None);
                            player.Position = position;
                        } else Log.Debug($"OnDying: Не смог воскресить {ev.Player.Nickname}. Причина: условие (IsConnected && IsDead)");
                        
                        playerRole.Remove(player);
                    }
                    Log.Debug($"OnDying: Список с ключом {ev.Player.Nickname} был удалён");
                    dictionary.Remove(ev.Player);
                }
                return;
            }

            if (ev.Attacker.Role.Type != RoleTypeId.Flamingo || Check(ev.Attacker)) return;

            foreach (Player pelicanFlamingo in TrackedPlayers)
            {
                if (ev.Attacker == pelicanFlamingo)
                {
                    if (dictionary.TryGetValue(ev.Attacker, out List<Player> deadPlayer))
                    {
                        if (!deadPlayer.Contains(ev.Player)) deadPlayer.Add(ev.Player);
                    }
                    else
                    {
                        dictionary.Add(ev.Attacker, new List<Player>() { ev.Player });
                    }

                    Log.Debug($"OnDying: {ev.Player.Nickname} добавлен в список playerRole и dictionary для пеликана {ev.Attacker.Nickname}");

                    ev.IsAllowed = false;
                    playerRole.Add(ev.Player, ev.Player.Role.Type);
                    ev.Player.Role.Set(RoleTypeId.Overwatch);
                }
            }
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            if (playerRole.ContainsKey(ev.Player))
            {
                ev.Player.Role.Set(RoleTypeId.Overwatch);
                Log.Debug($"OnVerified: {ev.Player.Nickname} находится в списке playerRole. Перевод в Overwatch");
            }
        }
    }
}
