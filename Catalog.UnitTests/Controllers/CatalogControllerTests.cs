using Catalog.API.Controllers;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.UnitTests.Controllers
{
    public class CatalogControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly CatalogController _sut;
        private readonly Mock<ILogger> _logger;

        public CatalogControllerTests()
        {
            _fixture = new Fixture();
            _repositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
            _logger = _fixture.Freeze<Mock<ILogger>>();
            _sut = new CatalogController(_repositoryMock.Object, _logger.Object);

        }

        [Fact]

        public async Task GetProducts_ShouldReturnOkResponse_WhenDataFound()
        {
            //Arrange
            var productsMock = _fixture.Create<Task<IEnumerable<Product>>>();
            _repositoryMock.Setup(x => x.GetProducts()).Returns(productsMock);

            //Act
            var result = await _sut.GetProducts().ConfigureAwait(false);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<IEnumerable<Product>>>();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            result.Result.As<OkObjectResult>().Value.Should().NotBeNull();
            _repositoryMock.Verify(x => x.GetProducts(), Times.Once());
        }

        [Fact]
        public async Task GetProducts_ShouldReturnNotFound_WhenDataNotFound()
        {
            //Arrange
            List<Product> response = null;
            _repositoryMock.Setup(x => x.GetProducts()).ReturnsAsync(response);

            //Act
            var result = await _sut.GetProducts().ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Result.As<OkObjectResult>().Value.Should().BeNull();
            _repositoryMock.Verify(x => x.GetProducts(), Times.Once());

        }

        [Fact]
        public async Task GetProductById_ShouldReturnOkResponse_WhenValidInput()
        {
            //Arrange
            var productsMock = _fixture.Create<Task<Product>>();
            var id = _fixture.Create<int>();
            _repositoryMock.Setup(x => x.GetProduct(Convert.ToString(id))).Returns(productsMock);

            //Act
            var result = await _sut.GetProductById(Convert.ToString(id)).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<Product>>();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            result.Result.As<OkObjectResult>().Value.Should().NotBeNull();
            _repositoryMock.Verify(x => x.GetProduct(Convert.ToString(id)), Times.Once());
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenNoDataFound()
        {
            //Arrange
            Product productsMock = null;
            int id = 0;
            _repositoryMock.Setup(x => x.GetProduct(Convert.ToString(id))).ReturnsAsync(productsMock);

            //Act
            var result = await _sut.GetProductById(Convert.ToString(id)).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
            _repositoryMock.Verify(x => x.GetProduct(Convert.ToString(id)), Times.Once());
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnOkResponse_WhenValidRequest()
        {
            //Arrange
            var request = _fixture.Create<Product>();
            var response = _fixture.Create<Task<Product>>();
            _repositoryMock.Setup(x => x.CreateProduct(request)).Returns(response);

            //Act
            var result = await _sut.CreateProduct(request).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<Product>>();
            result.Result.Should().BeAssignableTo<CreatedAtRouteResult>();
            _repositoryMock.Verify(x => x.CreateProduct(request), Times.Once());
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent_WhenRecordDeleted()
        {
            var id = _fixture.Create<int>();
            _repositoryMock.Setup(x => x.DeleteProduct(Convert.ToString(id))).ReturnsAsync(true);

            var result = await _sut.DeleteProductById(Convert.ToString(id)).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            _repositoryMock.Verify(x => x.DeleteProduct(Convert.ToString(id)), Times.Once());
        }


        [Fact]
        public async Task UpdateProduct_ShouldReturnOkResponse_WhenValidRequest()
        {
            //Arrange
            var request = _fixture.Create<Product>();
            _repositoryMock.Setup(x => x.UpdateProduct(request)).ReturnsAsync(true);

            //Act
            var result = await _sut.UpdateProduct(request).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            _repositoryMock.Verify(x => x.UpdateProduct(request), Times.Once());
        }

    }
}