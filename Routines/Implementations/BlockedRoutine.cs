using CV.Ads_Client.Configuration;

namespace CV.Ads_Client.Routines.Implementations
{
    public class BlockedRoutine : BaseNoopRoutine
    {
        public BlockedRoutine(IConfigurationManager configurationManager)
            : base(configurationManager)
        { }

        protected override string GetRoutineName() => nameof(BlockedRoutine);
    }
}
