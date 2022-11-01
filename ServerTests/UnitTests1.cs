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
        //[Fact]
        //public void GenerateRandomTokens()
        //{
        //    string[] tokens = new string[10];
        //    for(int i = 0;i<10;i++)
        //    {
        //        tokens[i] = TokenGenerator.GenerateToken();
        //    }
        //    // No groups larger than 1 element
        //    Assert.DoesNotContain(tokens.GroupBy(x => x), g => g.Count() > 1);
        //    
        //}
        [Fact]
        public void SecretKeyTest()
        {
            var fac = TestWebApplicationFactory.Create("");
            var client = fac.CreateClient();
            var secret = SecretKey.GetSecret();
            Assert.NotNull(secret.Key);
        }
    }
}
