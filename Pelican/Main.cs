
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;

namespace Pelican
{
    public class Main : Plugin<Config>
    {
        public static Main Plugin { get; private set; }
        public override string Author { get; } = "ShoulHate";
        public override string Name { get; } = "Pelican";

        public override void OnEnabled()
        {
            Plugin = this;

            CustomRole.RegisterRoles(false, overrideClass: new RolePelican());

            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            CustomRole.UnregisterRoles();

            base.OnDisabled();
        }
    }
}
