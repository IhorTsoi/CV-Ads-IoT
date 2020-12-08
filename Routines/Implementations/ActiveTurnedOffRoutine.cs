using CV.Ads_Client.Configuration;

namespace CV.Ads_Client.Routines.Implementations
{
    public class ActiveTurnedOffRoutine : BaseNoopRoutine
    {
        public ActiveTurnedOffRoutine(IConfigurationManager configurationManager)
            : base(configurationManager)
        { }

        protected override string GetRoutineName() => nameof(ActiveTurnedOffRoutine);
    }
}
