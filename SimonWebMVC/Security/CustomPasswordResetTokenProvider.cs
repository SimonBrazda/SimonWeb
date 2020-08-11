using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SimonWebMVC.Security
{
    public class CustomPasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomPasswordResetTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                                IOptions<DataProtectionTokenProviderOptions> options,
                                                ILogger<DataProtectorTokenProvider<TUser>> logger)
                                                : base(dataProtectionProvider, options, logger)
        {
        }
    }
}