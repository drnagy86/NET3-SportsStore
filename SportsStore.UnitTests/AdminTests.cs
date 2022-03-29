using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.Domain.Entities;
using System.Linq;
using System.Collections.Generic;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void IndexContainsAllProducts()
        {
            // arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1"
                },
                new Product
                {
                    ProductID = 2,
                    Name = "P2"
                },
                new Product
                {
                    ProductID = 3,
                    Name = "P3"
                }
            });

            // arrange - create the controller
            AdminController target = new AdminController(mock.Object);


            // action 
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void CanEditProduct()
        {
            // arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1"
                },
                new Product
                {
                    ProductID = 2,
                    Name = "P2"
                },
                new Product
                {
                    ProductID = 3,
                    Name = "P3"
                }
            });

            // arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // act

            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);

        }

       [TestMethod]
       public void CannotEditNonexistentProduct()
        {
            // arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1"
                },
                new Product
                {
                    ProductID = 2,
                    Name = "P2"
                },
                new Product
                {
                    ProductID = 3,
                    Name = "P3"
                }
            });

            // arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // act
            Product result = (Product)target.Edit(4).ViewData.Model;

            // assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CanSaveValidChanges()
        {
            // arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            Product product = new Product() { Name = "Test" };

            // arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            ActionResult result = target.Edit(product);

            // assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(product));

            // assert - check the method result type

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));


        }

        [TestMethod]
        public void CannotSaveInvalidChanges()
        {
            // arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            Product product = new Product() { Name = "Test" };

            // arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            ActionResult result = target.Edit(product);

            // assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            // assert - check the method result type

            Assert.IsInstanceOfType(result, typeof(ViewResult));


        }
    }
}
