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

using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace RurouniJones.Custodian.Configuration
{
    public sealed class Discord
    {
        // See https://discord.com/developers/docs/reference#snowflakes for more information
        private const ulong DISCORD_SNOWFLAKE_MINIMUM_VALUE = 10000000000000000; // 17 digits
        private const ulong DISCORD_SNOWFLAKE_MAXIMUM_VALUE = 999999999999999999; // 18 digits

        [Range(DISCORD_SNOWFLAKE_MINIMUM_VALUE, DISCORD_SNOWFLAKE_MAXIMUM_VALUE, ErrorMessage = "Guild ID must be specified with 17 or 18 digits")]
        public ulong GuildId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Token must be specified")]
        public string Token { get; set; }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
