using Exiled.API.Interfaces;

namespace Pelican
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public int MaxHealth { get; set; } = 3500;
        public string Description { get; set; } = "Test";
        public string CustomInfo { get; set; } = "Pelican";
        public string BroadcastRoleAdded { get; set; } = "<color=green>Пеликан вышел на охоту";
        public bool EnableBroadcastRoleAdded { get; set;} = false;
    }
}
