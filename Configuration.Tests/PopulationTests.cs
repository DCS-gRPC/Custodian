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
using YamlDotNet.Serialization;

namespace RurouniJones.Custodian.Configuration.Tests
{
    [TestClass]
    public class PopulationTests
    {
        private readonly IDeserializer Deserializer = new DeserializerBuilder().Build();

        const string VALID_CONFIGURATION = @"---
                                             Name: Example Group
                                             Discord:
                                               GuildId: 12345678901234567
                                               Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA
                                             GameServers:
                                               -
                                                 Name: Example Server
                                                 ShortName: EXS
                                                 Rpc:
                                                   Host: exs.example.com
                                                   Port: 50051";

        [TestMethod]
        public void ValidConfigurationFilePopulatesApplicationName()
        {
            var applicationConfiguration = Deserializer.Deserialize<Application>(VALID_CONFIGURATION);

            Assert.AreEqual("Example Group", applicationConfiguration.Name);
        }

        [TestMethod]
        public void ValidConfigurationFilePopulatesDiscordGuildId()
        {
            var applicationConfiguration = Deserializer.Deserialize<Application>(VALID_CONFIGURATION);

            Assert.AreEqual(12345678901234567UL, applicationConfiguration.Discord.GuildId);
        }

        [TestMethod]
        public void ValidConfigurationFilePopulatesDiscordToken()
        {
            var applicationConfiguration = Deserializer.Deserialize<Application>(VALID_CONFIGURATION);

            Assert.AreEqual("OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA",
                applicationConfiguration.Discord.Token);
        }

        [TestMethod]
        public void ValidConfigurationFilePopulatesFirstGameServerName()
        {
            var gameServerConfiguration = Deserializer.Deserialize<Application>(VALID_CONFIGURATION).GameServers[0];

            Assert.AreEqual("Example Server", gameServerConfiguration.Name);
        }

        [TestMethod]
        public void ValidConfigurationFilePopulatesFirstGameServerShortName()
        {
            var gameServerConfiguration = Deserializer.Deserialize<Application>(VALID_CONFIGURATION).GameServers[0];

            Assert.AreEqual("EXS", gameServerConfiguration.ShortName);
        }

        [TestMethod]
        public void ValidConfigurationFilePopulatesFirstGameServerRpcHost()
        {
            var rpcConfiguration = Deserializer.Deserialize<Application>(VALID_CONFIGURATION).GameServers[0].Rpc;

            Assert.AreEqual("exs.example.com", rpcConfiguration.Host);
        }

        [TestMethod]
        public void ValidConfigurationFilePopulatesFirstGameServerRpcPort()
        {
            var rpcConfiguration = Deserializer.Deserialize<Application>(VALID_CONFIGURATION).GameServers[0].Rpc;

            Assert.AreEqual(50051, rpcConfiguration.Port);
        }
    }
}
