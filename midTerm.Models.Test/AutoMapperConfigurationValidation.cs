using System;
using midTerm.Models.Profiles;
using midTerm.Models.Test.Internal;
using Xunit;

namespace midTerm.Models.Test
{
    public class AutoMapperConfigurationValidation
    {
        [Fact]
        public void IsValid()
        {
            // Arrange
            var configuration = AutoMapperModule.CreateMapperConfiguration<OptionProfile>();

            // Act/Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
