﻿using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.Dtos;
using Application.Dtos.AuthorDtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities.Core;
using Moq;

namespace TestProject.AuthorUnitTests
{
    [TestFixture]
    [Category("AuthorId/UnitTests/UpdateAuthor")]
    public class UpdateAuthorUnitTest
    {
        private UpdateAuthorCommandHandler _handler;
        private Mock<IGenericRepository<Author, Guid>> _mockRepository;
        private Mock<IMapper> _mockMapper;

        private static readonly Guid ExampleAuthorId = Guid.Parse("12345678-1234-1234-1234-1234567890ab");
        private static readonly UpdateAuthorDto ExampleAuthorDto = new()
        {
            AuthorId = ExampleAuthorId,
            FirstName = "Test",
            LastName = "Testsson"
        };

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IGenericRepository<Author, Guid>>();
            _mockMapper = new Mock<IMapper>();

            // Set up the mock repository to handle any AuthorId object with the same Id
            _mockRepository.Setup(repo => repo.UpdateAsync(It.Is<Author>(obj => obj.AuthorId == ExampleAuthorId)))
                           .ReturnsAsync((Author updatedAuthor) => updatedAuthor);

            _mockMapper.Setup(mapper => mapper.Map<Author>(It.IsAny<Author>()));
            _mockMapper.Setup(mapper => mapper.Map<Author>(It.IsAny<Author>()))
                       .Returns((Author author) => new Author
                       {
                           AuthorId = ExampleAuthorId,
                           FirstName = author.FirstName,
                           LastName = author.LastName
                       });


            _handler = new UpdateAuthorCommandHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Test]
        public async Task Handle_ValidInput_ReturnsAuthor()
        {
            // Arrange
            var command = new UpdateAuthorCommand(ExampleAuthorDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Data.FirstName, Is.EqualTo(ExampleAuthorDto.FirstName));
                Assert.That(result.Data.LastName, Is.EqualTo(ExampleAuthorDto.LastName));
            });
        }

        [Test]
        public async Task Handle_NullInput_ReturnsNull()
        {
            // Arrange
            UpdateAuthorDto authorToTest = null!;
            var command = new UpdateAuthorCommand(authorToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }


        [Test]
        public async Task Handle_MissingFirstName_ReturnsNull()
        {
            // Arrange
            var invalidAuthorDto = new UpdateAuthorDto
            {
                AuthorId = Guid.NewGuid(),
                FirstName = null!,
                LastName = "Testsson"
            };

            var command = new UpdateAuthorCommand(invalidAuthorDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}
