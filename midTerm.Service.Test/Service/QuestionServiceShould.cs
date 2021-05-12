using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using midTerm.Models.Models.Question;
using midTerm.Models.Profiles;
using midTerm.Service.Test.Internal;
using midTerm.Services.Services;
using Xunit;

namespace midTerm.Service.Test.Service
{
    public class QuestionServiceSHould
    : SqlLiteContext
    {
        private readonly IMapper _mapper;
        private readonly QuestionService _service;

        public QuestionServiceSHould()
        : base(true)
        {
            if (_mapper == null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(typeof(QuestionProfile));
                }).CreateMapper();
                _mapper = mapper;
            }
            _service = new QuestionService(DbContext, _mapper);
        }

        [Fact]
        public async Task GetByQuestionId()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.GetById(expected);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Models.Models.Question.QuestionModelExtended>();
            result.Id.Should().Be(expected);
        }

        [Fact]
        public async Task GetAllQuestions()
        {
            // Arrange
            var expected = 3;

            // Act
            var result = await _service.Get();

            // Assert
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<Models.Models.Question.QuestionModelBase>>();
        }
        [Fact]
        public async Task GetFullQuestions()
        {
            // Arrange
            var expected = 3;

            // Act
            var result = await _service.GetFull();

            // Assert
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<Models.Models.Question.QuestionModelExtended>>();
        }

        [Fact]
        public async Task InsertNewQuestion()
        {
            // Arrange
            var question = new QuestionCreateModel
            {
                Text = "In what programming language are we writing right now",
                Description = "Languages are important to understand"
            };

            // Act
            var result = await _service.Insert(question);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<QuestionModelBase>();
            result.Id.Should().NotBe(0);
        }

        [Fact]
        public async Task UpdateQuestion()
        {
            // Arrange
            var question = new QuestionUpdateModel
            {
                Id = 1,
                Text = "In what programming language are we writing right now",
                Description = "Languages are important to understand"
            };

            // Act
            var result = await _service.Update(question);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<QuestionModelBase>();
            result.Id.Should().Be(question.Id);
            result.Text.Should().Be(question.Text);
            result.Description.Should().Be(question.Description);

        }

        [Fact]
        public async Task ThrowExceptionOnUpdateQuestion()
        {
            // Arrange
            var match = new QuestionUpdateModel
            {
                Id = 10,
                Text = "In what programming language are we writing right now",
                Description = "Languages are important to understand"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.Update(match));
            Assert.Equal("Question not found", exception.Message);

        }

        [Fact]
        public async Task DeleteMatch()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.Delete(expected);
            var question = await _service.GetById(expected);

            // Assert
            result.Should().Be(true);
            question.Should().BeNull();
        }
    }
}
