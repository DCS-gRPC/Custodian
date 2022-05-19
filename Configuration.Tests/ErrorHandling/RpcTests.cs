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
    public class RpcTests
    {
        private readonly IDeserializer Deserializer = new DeserializerBuilder().Build();

        # region Host Testing

        [TestMethod]
        public void WhenRpcHostKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Port: 50051";

            var rpcConfiguration = Deserializer.Deserialize<Rpc>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(rpcConfiguration, "Rpc Host must be specified");
        }

        [TestMethod]
        public void WhenRpcHostIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Host: ~
                                           Port: 50051";

            var rpcConfiguration = Deserializer.Deserialize<Rpc>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(rpcConfiguration, "Rpc Host must be specified");
        }

        [TestMethod]
        public void WhenRpcHostIsEmpty_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Host: ''
                                           Port: 50051";

            var rpcConfiguration = Deserializer.Deserialize<Rpc>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(rpcConfiguration, "Rpc Host must be specified");
        }

        #endregion

        #region Rpc Port Testing

        [TestMethod]
        public void WhenRpcPortKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Host: exs.example.com";

            var rpcConfiguration = Deserializer.Deserialize<Rpc>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(rpcConfiguration, "Rpc Port must be set between 1025 and 65535 inclusive");
        }

        [TestMethod]
        public void WhenRpcPortIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Host: exs.example.com
                                           Port: ~";

            var rpcConfiguration = Deserializer.Deserialize<Rpc>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(rpcConfiguration, "Rpc Port must be set between 1025 and 65535 inclusive");
        }

        [TestMethod]
        public void WhenRpcPortIsBelowRange_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Host: exs.example.com
                                           Port: 1024";

            var rpcConfiguration = Deserializer.Deserialize<Rpc>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(rpcConfiguration, "Rpc Port must be set between 1025 and 65535 inclusive");
        }

        [TestMethod]
        public void WhenRpcPortIsAboveRange_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Host: exs.example.com
                                           Port: 65536";

            var rpcConfiguration = Deserializer.Deserialize<Rpc>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(rpcConfiguration, "Rpc Port must be set between 1025 and 65535 inclusive");
        }

        #endregion
    }
}
