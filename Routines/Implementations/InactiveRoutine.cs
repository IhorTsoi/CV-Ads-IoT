using CV.Ads_Client.Configuration;

namespace CV.Ads_Client.Routines.Implementations
{
    public class InactiveRoutine : BaseNoopRoutine
    {
        public InactiveRoutine(IConfigurationManager configurationManager)
            : base(configurationManager)
        { }

        protected override string GetRoutineName() => nameof(InactiveRoutine);
    }
}
