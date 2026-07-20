namespace SafePharma.Common
{
    public interface IOtpDeliveryChannel
    {
        public string ChannelName { get; }
        Task<GeneralResult> SendAsync( string phoneNumber ,string otpCode , CancellationToken ct=default);
        
    }
}
