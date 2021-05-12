using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using midTerm.Controllers;
using midTerm.Models.Models.Option;
using midTerm.Models.Models.Question;
using midTerm.Services.Abstractions;
using Moq;
using Xunit;

namespace midTerm.Controller.Test
{
    public class QuestionControllerShould
    {
        private readonly Mock<IQuestionService> _mockService;
        private readonly QuestionsController _controller;

        public QuestionControllerShould()
        {
            _mockService = new Mock<IQuestionService>();

            _controller = new QuestionsController(_mockService.Object);
        }
        [Fact]
        public async Task ReturnQuestionsWhenHasData()
        {
            // Arrange
            int expectedCount = 10;
            var questions = new Faker<QuestionModelBase>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate(expectedCount);

            _mockService.Setup(x => x.Get())
                .ReturnsAsync(questions)
                .Verifiable();

            // Act
            var result = await _controller.Get();

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<List<QuestionModelBase>>().Subject.Count().Should().Be(expectedCount);
        }
        [Fact]
        public async Task ReturnExtendedQuestionByIdWhenHasData()
        {
            // Arrange
            int expectedId = 1;

            var options = new Faker<OptionBaseModel>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Order, v => v.Random.Number());
            var questions = new Faker<QuestionModelExtended>()
                .RuleFor(s => s.Id, v => ++v.IndexVariable)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .RuleFor(s => s.Options, v => options.Generate(3).ToList())
                .Generate(6);

            _mockService.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(questions.Find(x => x.Id == expectedId))
                .Verifiable();

            // Act
            var result = await _controller.GetById(expectedId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<QuestionModelExtended>().Subject.Id.Should().Be(expectedId);
        }
        [Fact]
        public async Task ReturnCreatedQuestionOnCreateWhenCorrectModel()
        {
            // Arrange
            var question = new Faker<QuestionCreateModel>()
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();
            var expected = new Faker<QuestionModelBase>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<QuestionCreateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Post(question);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>();

            var model = result as CreatedAtRouteResult;
            model?.Value.Should().Be(1);
        }

        [Fact]
        public async Task ReturnConflictOnCreateWhenRepositoryError()
        {
            // Arrange
            var question = new Faker<QuestionCreateModel>()
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<QuestionCreateModel>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            // Act
            var result = await _controller.Post(question);

            // Assert
            result.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public async Task ReturnBadRequestOnCreateWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("someFakeError", "fakeError message");
            var question = new Faker<QuestionCreateModel>()
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();
            var expected = new Faker<QuestionModelBase>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<QuestionCreateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Post(question);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ReturnQuestionOnUpdateWhenCorrectModel()
        {
            // Arrange
            var question = new Faker<QuestionUpdateModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();
            var expected = new Faker<QuestionModelBase>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<QuestionUpdateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Put(question.Id, question);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnNoContentOnUpdateWhenRepositoryError()
        {
            // Arrange
            var question = new Faker<QuestionUpdateModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();
            var expected = new Faker<QuestionModelBase>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<QuestionUpdateModel>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            // Act
            var result = await _controller.Put(question.Id, question);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ReturnBadRequestOnUpdateWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FakeError", "FakeError message");
            var question = new Faker<QuestionUpdateModel>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();
            var expected = new Faker<QuestionModelBase>()
                .RuleFor(s => s.Id, 1)
                .RuleFor(s => s.Text, v => v.Random.Words())
                .RuleFor(s => s.Description, v => v.Random.Words())
                .Generate();

            _mockService.Setup(x => x.Update(It.IsAny<QuestionUpdateModel>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Put(question.Id, question);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ReturnOkWhenDeletedData()
        {
            // Arrange
            int id = 1;
            bool expected = true;

            _mockService.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnOkFalseWhenNoData()
        {
            // Arrange
            int id = 1;
            bool expected = false;

            _mockService.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnBadResultWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("FakeError", "FakeError message");

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
