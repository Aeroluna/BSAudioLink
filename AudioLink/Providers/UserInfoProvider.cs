using System.Threading.Tasks;
using JetBrains.Annotations;
using Zenject;
#if !V1_29_1
using System.Threading;
#endif

namespace AudioLink.Providers
{
    internal class UserInfoProvider : IInitializable
    {
        private readonly IPlatformUserModel _platformUserModel;
        private readonly Scripts.AudioLink _audioLink;

        [UsedImplicitly]
        private UserInfoProvider(IPlatformUserModel platformUserModel, Scripts.AudioLink audioLink)
        {
            _platformUserModel = platformUserModel;
            _audioLink = audioLink;
        }

        public void Initialize()
        {
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
    }
}
