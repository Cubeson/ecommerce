using Server;
using Server.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests
{
    public class UnitTests1
    {

        [Fact]
        public void JWTSettingsTest()
        {
            var fac = TestWebApplicationFactory.Create("");
            fac.CreateClient();
            var secret = JWTSingleton.Get();
            Assert.NotNull(secret.Key);
        }
    }
}
