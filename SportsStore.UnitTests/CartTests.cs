using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.Domain.Entities;
using System.Linq;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void CanAddToCart()
        {
            // arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1",
                    Category = "Apples"
                },
            }.AsQueryable());

            // arrange - create a cart
            Cart cart = new Cart();

            // Arrange- create the controller
            CartController target = new CartController(mock.Object, null);

            // Act - add a product to the cart
            target.AddToCart(cart, 1, null);

            // Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void AddingProductToCartGoesToCartScreen()
        {
            // arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1",
                    Category = "Apples"
                },
            }.AsQueryable());

            // arrange - create a cart
            Cart cart = new Cart();

            // Arrange- create the controller
            CartController target = new CartController(mock.Object, null);

            // Act - add a product to the cart
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            // Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");

        }

        [TestMethod]
        public void CanViewCarContents()
        {
            // arrange - create a cart
            Cart cart = new Cart();

            // arrange - create the controller
            CartController target = new CartController(null, null);

            // act - call the Index action mthod
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            // assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");

        }

        [TestMethod]
        public void CannotCheckoutEmptyCart()
        {
            // arrange -  create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // arrange - create an empty cart
            Cart cart = new Cart();

            // arrange - create shipping detailss
            ShippingDetails shippingDetails = new ShippingDetails();

            // arrange- create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            // act
            ViewResult result = target.Checkout(cart, shippingDetails);

            // assert- check that the method is returning the default view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);


        }

        [TestMethod]
        public void CannotCheckoutInvalidShippingDetails()
        {
            // arrange -  create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // arrange - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // arrange- create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            // arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");

            // act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // assert- check that the order has not been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            // assert- check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);

            // asssert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_CheckoutAndSubmitOrder()
        {
            // arrange -  create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // arrange- create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            
            

            // act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // assert- check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());

            // assert- check that the method is returning the completed view
            Assert.AreEqual("Completed", result.ViewName);

            // asssert - check that I am passing a valid model to the view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);

        }
    }
}
