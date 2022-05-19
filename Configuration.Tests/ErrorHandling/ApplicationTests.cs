/* 
Custodian is a DCS server administration tool for Discord
Copyright (C) 2022 Jeffrey Jones

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RurouniJones.Custodian.Configuration.Tests.ErrorHandling.CustomAssertions;
using YamlDotNet.Serialization;

namespace RurouniJones.Custodian.Configuration.Tests.ErrorHandling
{
    [TestClass]
    public class ApplicationTests
    {
        private readonly IDeserializer Deserializer = new DeserializerBuilder().Build();

        # region Name Testing

        [TestMethod]
        public void WhenApplicationNameKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Discord:
                                             GuildId: 12345678901234567
                                             Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA
                                           GameServers:
                                             -
                                               Name: Example
                                               ShortName: EXS
                                               Rpc:
                                                 Host: exs.example.com
                                                 Port: 50051";

            var applicationConfiguration = Deserializer.Deserialize<Application>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(applicationConfiguration, "Application Name must be specified");
        }

        [TestMethod]
        public void WhenApplicationNameIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: ~
                                           Discord:
                                             GuildId: 12345678901234567
                                             Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA
                                           GameServers:
                                             -
                                               Name: Example
                                               ShortName: EXS
                                               Rpc:
                                                 Host: exs.example.com
                                                 Port: 50051";

            var applicationConfiguration = Deserializer.Deserialize<Application>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(applicationConfiguration, "Application Name must be specified");
        }

        [TestMethod]
        public void WhenApplicationNameIsEmpty_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: ''
                                           Discord:
                                             GuildId: 12345678901234567
                                             Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA
                                           GameServers:
                                             -
                                               Name: Example
                                               ShortName: EXS
                                               Rpc:
                                                 Host: exs.example.com
                                                 Port: 50051";

            var applicationConfiguration = Deserializer.Deserialize<Application>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(applicationConfiguration, "Application Name must be specified");
        }

        #endregion

        #region GameServer List Testing

        [TestMethod]
        public void WhenGameServerKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: Example
                                           Discord:
                                             GuildId: 12345678901234567
                                             Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA";

            var applicationConfiguration = Deserializer.Deserialize<Application>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(applicationConfiguration, "GameServers must define at least one GameServer");
        }

        [TestMethod]
        public void WhenGameServerKeyIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: Example
                                           Discord:
                                             GuildId: 12345678901234567
                                             Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA
                                           GameServers: ~";

            var applicationConfiguration = Deserializer.Deserialize<Application>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(applicationConfiguration, "GameServers must define at least one GameServer");
        }

        #endregion
    }
}
