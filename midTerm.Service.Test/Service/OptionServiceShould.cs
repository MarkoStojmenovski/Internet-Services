using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using midTerm.Models.Models.Option;
using midTerm.Models.Profiles;
using midTerm.Service.Test.Internal;
using midTerm.Services.Services;
using Xunit;

namespace midTerm.Service.Test.Service
{
    public class OptionServiceShould : SqlLiteContext
    {
        private readonly IMapper _mapper;
        private readonly OptionService _service;

        public OptionServiceShould()
        : base(true)
        {
            if (_mapper == null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(typeof(OptionProfile));
                }).CreateMapper();
                _mapper = mapper;
            }
            _service = new OptionService(DbContext, _mapper);
        }
        [Fact]
        public async Task GetByOptionId()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.GetById(expected);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Models.Models.Option.OptionModelExtended>();
            result.Id.Should().Be(expected);
        }

        [Fact]
        public async Task GetByQuestionId()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.GetByQuestionId(expected);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<Models.Models.Option.OptionModelExtended>>();
        }

        [Fact]
        public async Task GetAllOptions()
        {
            // Arrange
            var expected = 3;

            // Act
            var result = await _service.Get();

            // Assert
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<Models.Models.Option.OptionBaseModel>>();
        }

        [Fact]
        public async Task InsertNewOption()
        {
            // Arrange
            var option = new OptionCreateModel
            {
                Text = "Option 1",
                QuestionId = 1
            };

            // Act
            var result = await _service.Insert(option);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OptionBaseModel>();
            result.Id.Should().NotBe(0);
        }

        [Fact]
        public async Task UpdateQuestion()
        {
            // Arrange
            var option = new OptionUpdateModel
            {
                Id = 1,
                Text = "Option 1",
                QuestionId = 1
            };

            // Act
            var result = await _service.Update(option);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OptionBaseModel>();
            result.Id.Should().Be(option.Id);
            result.Text.Should().Be(option.Text);
            result.QuestionId.Should().Be(option.QuestionId);

        }

        [Fact]
        public async Task ThrowExceptionOnUpdateQuestion()
        {
            // Arrange
            var option = new OptionUpdateModel
            {
                Id = 10,
                Text = "Option 1",
                QuestionId = 1
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.Update(option));
            Assert.Equal("Option not found", exception.Message);

        }

        [Fact]
        public async Task DeleteMatch()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.Delete(expected);
            var option = await _service.GetById(expected);

            // Assert
            result.Should().Be(true);
            option.Should().BeNull();
        }
    }
}
