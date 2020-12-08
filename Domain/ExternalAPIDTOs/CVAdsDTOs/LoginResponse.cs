using CV.Ads_Client.Domain.Constants;
using System;

namespace CV.Ads_Client.Domain.ExternalAPIDTOs.CVAdsDTOs
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public SmartDeviceMode Mode { get; set; }
        public bool IsTurnedOn { get; set; }
        public bool IsCaching { get; set; }

        public override string ToString() =>
            $"Mode: {Mode}. Caching: {IsCaching}. Screen is turned on: {IsTurnedOn}.";
    }
}
