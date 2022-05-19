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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RurouniJones.Custodian.Configuration.Tests.ErrorHandling.CustomAssertions
{
    internal static class AssertErrorMessage
    {
        public static void ContainsErrorMessage(this Assert _, object model, string message)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            var containsErrorMessage = validationResults.Any(x => x.ErrorMessage.Contains(message));
            #pragma warning restore CS8602 // Dereference of a possibly null reference.

            if(!containsErrorMessage)
                throw new AssertFailedException($"Expect to find eror message \"{message}\" but was not found");
        }
    }
}
