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
    public class DiscordTests
    {
        private readonly IDeserializer Deserializer = new DeserializerBuilder().Build();

        # region Guild ID Testing

        [TestMethod]
        public void WhenGuildIdKeyIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA";

            var discordConfiguration = Deserializer.Deserialize<Discord>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(discordConfiguration, "Guild ID must be specified with 17 or 18 digits");
        }

        [TestMethod]
        public void WhenGuildIdIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           GuildId: ~
                                           Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA";

            var discordConfiguration = Deserializer.Deserialize<Discord>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(discordConfiguration, "Guild ID must be specified with 17 or 18 digits");
        }

        [TestMethod]
        public void WhenGuildIdIsLessThan17Digits_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           GuildId: 1
                                           Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA";

            var discordConfiguration = Deserializer.Deserialize<Discord>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(discordConfiguration, "Guild ID must be specified with 17 or 18 digits");
        }

        [TestMethod]
        public void WhenGuildIdIsGreaterThan18Digits_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           GuildId: 10000000000000000000
                                           Token: OTg2ODA8Njk5NTI3NzgyNDAw.ADfi-f.oRKJVdqjvtpo1iXR57iHkIrpLgyr1wTnH0jfzA";

            var discordConfiguration = Deserializer.Deserialize<Discord>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(discordConfiguration, "Guild ID must be specified with 17 or 18 digits");
        }

        # endregion

        # region Token Testing

        [TestMethod]
        public void WhenTokenIsMissing_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           GuildId: 12345678901234567";

            var discordConfiguration = Deserializer.Deserialize<Discord>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(discordConfiguration, "Token must be specified");
        }

        [TestMethod]
        public void WhenTokenIsNull_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           GuildId: 12345678901234567
                                           Token: ~";

            var discordConfiguration = Deserializer.Deserialize<Discord>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(discordConfiguration, "Token must be specified");
        }

        [TestMethod]
        public void WhenTokenIsEmpty_ThenThrowValidationException()
        {
            const string CONFIGURATION = @"---
                                           GuildId: 12345678901234567
                                           Token: ''";

            var discordConfiguration = Deserializer.Deserialize<Discord>(CONFIGURATION);

            Assert.That.ContainsErrorMessage(discordConfiguration, "Token must be specified");
        }

        # endregion
    }
}
