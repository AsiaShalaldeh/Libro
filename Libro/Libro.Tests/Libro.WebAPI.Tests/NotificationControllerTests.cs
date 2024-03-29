﻿using Libro.Domain.Interfaces.IServices;
using Libro.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Tests.Libro.WebAPI.Tests
{
    public class NotificationControllerTests
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<NotificationController>> _loggerMock;
        private readonly NotificationController _controller;

        public NotificationControllerTests()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<NotificationController>>();
            _controller = new NotificationController(_notificationServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SendOverdueNotification_OverdueBooksExist_ReturnsOkResult()
        {
            // Arrange
            _notificationServiceMock.Setup(x => x.SendOverdueNotification()).ReturnsAsync(true);

            // Act
            var result = await _controller.SendOverdueNotification();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Overdue notification sent successfully", okResult.Value);
        }

        [Fact]
        public async Task SendOverdueNotification_NoOverdueBooksExist_ReturnsNotFoundResult()
        {
            // Arrange
            _notificationServiceMock.Setup(x => x.SendOverdueNotification()).ReturnsAsync(false);

            // Act
            var result = await _controller.SendOverdueNotification();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No Overdue Books Found", notFoundResult.Value);
        }
    }
}
