using System.Threading.Tasks;
using JetBrains.Annotations;
using Zenject;
#if LATEST
using OculusStudios.Platform.Core;
#endif
#if !V1_29_1
using System.Threading;
#endif

namespace AudioLink.Providers
{
    internal class UserInfoProvider : IInitializable
    {
#if LATEST
        private readonly IPlatform _platformUserModel;
#else
        private readonly IPlatformUserModel _platformUserModel;
#endif
        private readonly Scripts.AudioLink _audioLink;

        [UsedImplicitly]
#if LATEST
        private UserInfoProvider(IPlatform platformUserModel, Scripts.AudioLink audioLink)
#else
        private UserInfoProvider(IPlatformUserModel platformUserModel, Scripts.AudioLink audioLink)
#endif
        {
            _platformUserModel = platformUserModel;
            _audioLink = audioLink;
        }

        public void Initialize()
        {
#if LATEST
            IPlatformUser userInfo = _platformUserModel.user;
            _audioLink.SetLocalPlayerName(userInfo.displayName);
        }
#else
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
    #if V1_29_1
            UserInfo userInfo = await _platformUserModel.GetUserInfo();
    #else
            UserInfo userInfo = await _platformUserModel.GetUserInfo(CancellationToken.None);
    #endif
            _audioLink.SetLocalPlayerName(userInfo.userName);
        }
#endif
    }
}
