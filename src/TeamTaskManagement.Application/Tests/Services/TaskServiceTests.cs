
using Moq;
using TeamTaskManagement.Application.DTOs.Tasks;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Application.Services;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Entities.Enums;
using TeamTaskManagement.Domain.Exceptions;
using Xunit;

namespace TeamTaskManagement.Application.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockTeamRepository = new Mock<ITeamRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _taskService = new TaskService(_mockTaskRepository.Object, _mockTeamRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task GetTeamTasksAsync_UserNotInTeam_ThrowsDomainException()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockTeamRepository.Setup(x => x.IsUserInTeamAsync(userId, teamId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _taskService.GetTeamTasksAsync(teamId, userId));

            Assert.Equal("Access denied. User is not a member of this team.", exception.Message);
        }

        [Fact]
        public async Task CreateTaskAsync_ValidRequest_ReturnsTaskDto()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new CreateTaskRequest("Test Task", "Description", DateTime.UtcNow.AddDays(7), null);

            var createdTask = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                CreatedByUserId = userId,
                TeamId = teamId,
                CreatedByUser = new User { FirstName = "John", LastName = "Doe" }
            };

            _mockTeamRepository.Setup(x => x.IsUserInTeamAsync(userId, teamId))
                .ReturnsAsync(true);
            _mockTaskRepository.Setup(x => x.CreateAsync(It.IsAny<TaskEntity>()))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _taskService.CreateTaskAsync(teamId, request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(teamId, result.TeamId);
        }
    }
}
