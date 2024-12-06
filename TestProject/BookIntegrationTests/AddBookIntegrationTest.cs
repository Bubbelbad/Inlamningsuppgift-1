﻿using Application.Commands.AddBook;
using Application.Dtos;
using Application.Interfaces.RepositoryInterfaces;
using AutoMapper;
using Domain.Model;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace TestProject.BookIntegrationTests
{
    [TestFixture]
    [Category("Book/Integration/AddBook")]
    public class AddBookIntegrationTest
    {
        private AddBookCommandHandler _handler;
        private RealDatabase _database;
        private IMapper _mapper;
        private IBookRepository _repository;

        private static readonly Guid ExampleBookId = new Guid("12345678-1234-1234-1234-1234567890ab");
        private static readonly AddBookDto ExampleBookDto = new("Test", "Testsson", "An example book for Testing");

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<RealDatabase>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _database = new RealDatabase(options);

            // Initialize the repository with the in-memory database
            _repository = new BookRepository(_database);

            // Set up AutoMapper with actual configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddAuthorDto, Author>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ExampleBookId));
            });
            _mapper = config.CreateMapper();

            // Initialize the handler with the actual repository and mapper
            _handler = new AddBookCommandHandler(_repository, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Database.EnsureDeleted();
            _database.Dispose();
        }

        [Test]
        public async Task Handle_ValidInput_ReturnsBook()
        {
            // Arrange
            var command = new AddBookCommand(ExampleBookDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data.Description, Is.EqualTo(ExampleBookDto.Description));
        }

        [Test]
        public async Task Handle_NullInput_ReturnsNull()
        {
            // Arrange
            AddBookDto bookToTest = null!; // Use null-forgiving operator to explicitly indicate null
            var command = new AddBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }

        [Test]
        public async Task Handle_MissingTitle_ReturnsNull()
        {
            // Arrange
            AddBookDto bookToTest = new(null!, "Test Testsson", "An example book for Testing"); // Use null-forgiving operator to explicitly indicate null
            var command = new AddBookCommand(bookToTest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.EqualTo(false));
        }
    }
}