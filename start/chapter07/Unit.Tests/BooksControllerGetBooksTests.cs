using Xunit;
using AutoFixture.Xunit2;
using FluentAssertions.AspNetCore.Mvc;
using books.Controllers;
using books.Services;
using books.Models;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

[Collection("BooksController Tests")]
public class BooksControllerGetBooksTests
{
    [Theory]
    [AutoNSubstituteData]
    public async Task GetBooks_ReturnsOk_WhenBooksExist(
        PagedResult<BookDTO> pagedBooks,
        IBooksService booksService)
    {
        // Arrange
        booksService.GetBooksAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<IUrlHelper>())
                    .Returns(pagedBooks);
        var controller = new BooksController(booksService);

        // Act
        var result = await controller.GetBooks(pageSize: 5, lastId: 0);

        // Temporary debug step to check the actual type
        result.Should().BeOfType<OkObjectResult>(); // This checks if the correct type is returned

        // Assert the contents
        result.Should().BeOkObjectResult()
              .WithValue(pagedBooks.Items)
              .WithStatusCode(StatusCodes.Status200OK);
    }
}
