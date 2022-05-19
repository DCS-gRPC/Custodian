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
    public class GameServerTests
    {
        private readonly IDeserializer Deserializer = new DeserializerBuilder().Build();

        # region Name Testing

        [TestMethod]
        public void WhenGameServerNameKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           ShortName: EXS
                                           Rpc:
                                             Host: exs.example.com
                                             Port: 50051";

            var gameServerConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(gameServerConfiguration, "GameServer Name must be specified");
        }

        [TestMethod]
        public void WhenGameServerNameIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: ~
                                           ShortName: EXS
                                           Rpc:
                                             Host: exs.example.com
                                             Port: 50051";

            var gameServerConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(gameServerConfiguration, "GameServer Name must be specified");
        }

        [TestMethod]
        public void WhenGameServerNameIsEmpty_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: ''
                                           ShortName: EXS
                                           Rpc:
                                             Host: exs.example.com
                                             Port: 50051";

            var gameServerConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(gameServerConfiguration, "GameServer Name must be specified");
        }

        #endregion

        #region ShortName Testing

        [TestMethod]
        public void WhenGameServerShortNameKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: Example
                                           Rpc:
                                             Host: exs.example.com
                                             Port: 50051";

            var gameServerConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(gameServerConfiguration, "GameServer ShortName must be specified");
        }

        [TestMethod]
        public void WhenGameServerShortNameIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: Example
                                           ShortName: ~
                                           Rpc:
                                             Host: exs.example.com
                                             Port: 50051";

            var gameServerConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(gameServerConfiguration, "GameServer ShortName must be specified");
        }

        [TestMethod]
        public void WhenGameServerShortNameIsEmpty_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: Example
                                           ShortName: ''
                                           Rpc:
                                             Host: exs.example.com
                                             Port: 50051";

            var gameServerConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(gameServerConfiguration, "GameServer ShortName must be specified");
        }

        #endregion

        #region Rpc Testing

        [TestMethod]
        public void WhenGameServerRpcKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: Example
                                           ShortName: EXS";

            var applicationConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(applicationConfiguration, "GameServer Rpc must be specified");
        }

        [TestMethod]
        public void WhenGameServerRpcKeyIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Name: Example
                                           ShortName: EXS
                                           Rpc: ~";

            var applicationConfiguration = Deserializer.Deserialize<GameServer>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(applicationConfiguration, "GameServer Rpc must be specified");
        }

        #endregion

    }
}
