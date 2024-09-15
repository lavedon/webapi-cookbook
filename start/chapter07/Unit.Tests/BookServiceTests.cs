using Xunit;
using NSubstitute;
using AutoFixture.Xunit2;
using books.Repositories;
using books.Models;
using books.Services;


namespace Tests.Services;

// BooksServiceTests.cs

public class BooksServiceTests
{
		[Fact]
        public async Task GetBookById_ReturnsBookDTO_WhenBookExists()
        {
            // Arrange
            int testBookId = 1;
            var bookFromRepository = new Book
            {
                Id = testBookId,
                Title = "Test Book",
                Author = "Test Author",
                PublicationDate = new DateTime(2020, 1, 1),
                ISBN = "1234567890123",
                Genre = "Test Genre",
                Summary = "Test Summary"
            };

            var expectedBookDto = new BookDTO
            {
                Id = testBookId,
                Title = "Test Book",
                Author = "Test Author",
                PublicationDate = new DateTime(2020, 1, 1),
                ISBN = "1234567890123",
                Genre = "Test Genre",
                Summary = "Test Summary"
            };

        var repository = new FakeBooksRepository(bookFromRepository);
        
        var service = new BooksService(repository);

        // Act
        var result = await service.GetBookByIdAsync(testBookId);

                    // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBookDto.Id, result.Id);
            Assert.Equal(expectedBookDto.Title, result.Title);
            Assert.Equal(expectedBookDto.Author, result.Author);
            Assert.Equal(expectedBookDto.PublicationDate, result.PublicationDate);
            Assert.Equal(expectedBookDto.ISBN, result.ISBN);
            Assert.Equal(expectedBookDto.Genre, result.Genre);
            Assert.Equal(expectedBookDto.Summary, result.Summary);
        }

        [Theory]
        [InlineAutoData(1)]
        [InlineAutoData(2)]
        [InlineAutoData(3)]
        public async Task GetBookByIdTheory_ReturnsBookDTO_WhenBookExists(int testBookId)
        {
            // Difference here is no local variable for id is set
            // Arrange
            var bookFromRepository = new Book
            {
                Id = testBookId,
                Title = "Test Book",
                Author = "Test Author",
                PublicationDate = new DateTime(2020, 1, 1),
                ISBN = "1234567890123",
                Genre = "Test Genre",
                Summary = "Test Summary"
            };

            var expectedBookDto = new BookDTO
            {
                Id = testBookId,
                Title = "Test Book",
                Author = "Test Author",
                PublicationDate = new DateTime(2020, 1, 1),
                ISBN = "1234567890123",
                Genre = "Test Genre",
                Summary = "Test Summary"
            };

            var repository = new FakeBooksRepository(bookFromRepository);
            var service = new BooksService(repository);

            // Act
            var result = await service.GetBookByIdAsync(testBookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBookDto.Id, result.Id);
            Assert.Equal(expectedBookDto.Title, result.Title);
            Assert.Equal(expectedBookDto.Author, result.Author);
            Assert.Equal(expectedBookDto.PublicationDate, result.PublicationDate);
            Assert.Equal(expectedBookDto.ISBN, result.ISBN);
            Assert.Equal(expectedBookDto.Genre, result.Genre);
            Assert.Equal(expectedBookDto.Summary, result.Summary);
        }

        public class FakeBooksRepository : IBooksRepository
        {
            private readonly Book _bookToReturn;

            public FakeBooksRepository(Book bookToReturn)
            {
                _bookToReturn = bookToReturn;
            }

            public Task<Book?> GetBookByIdAsync(int id)
            {

                if (_bookToReturn != null && _bookToReturn.Id == id)
                {
                    return Task.FromResult<Book?>(_bookToReturn);
                }

                return Task.FromResult<Book?>(null);
            }

            public Task<IReadOnlyCollection<Book>> GetBooksAsync(int pageSize, int lastId)
            {
                throw new NotImplementedException("GetBooksAsync is not implemented in FakeBooksRepository.");

            }

            public Task<Book> CreateBookAsync(Book book)
            {
                throw new NotImplementedException("CreateBookAsync is not implemented in FakeBooksREpository.");
            }
        }
}


