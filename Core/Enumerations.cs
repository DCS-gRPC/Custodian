﻿/* 
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

namespace RurouniJones.Custodian.Core
{
    public class Enumerations
    {
        public enum Coalition
        {
            Bluefor = 3,
            RedFor = 2,
            Neutral = 1
        }

        public enum BanPeriodUnit
        {
            Minutes,
            Hours,
            Days
        }
    }
}
