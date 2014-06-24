using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.Library.Algorithm
{
    class FacebookDatabaseFillerAlgorithm
    {
        private readonly string _facebookAppId;
        private readonly string _facebookAppSecret;
        private readonly FacebookClient _facebookClient;
        public FacebookDatabaseFillerAlgorithm()
        {
            _facebookAppId = "755768117817489";
            _facebookAppSecret = "cd35bedfe02676d7952acf0bdce96374";
        }

        private void InitializeFacebookClient()
        {
            dynamic result = _facebookClient.Get("oauth/access_token", new
            {
                client_id = _facebookAppId,
                client_secret = _facebookAppSecret,
                grant_type = "client_credentails"
            });

            _facebookClient.AccessToken = result.access_token;
        }

        public void FillDatabase()
        {
            InitializeFacebookClient();

            
        }
    }
}
