using Booking.Application.Doctors.Command.UpdateProfilePhoto;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using Booking.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace tests.Booking.Application.UnitTests.Doctors
{
    public class UpdateDoctorPhotoCommandHandlerTests
    {
        private readonly BookingDbContext _context;
        private readonly Mock<IFileStorageService> _fileStorageMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly UpdateDoctorPhotoCommandHandler _handler;

        public UpdateDoctorPhotoCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new BookingDbContext(options);

            _fileStorageMock = new Mock<IFileStorageService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new UpdateDoctorPhotoCommandHandler(
                _context,
                _fileStorageMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldUpdatePhotoUrl_WhenDoctorExists()
        {
            var userId = Guid.NewGuid().ToString();
            var specialtyId = Guid.NewGuid();

            var doctor = new Doctor("Gregory", "House", specialtyId, true)
            {
                UserId = userId
            };

            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

            var expectedUrl = "/uploads/doctors/cool-photo.jpg";

            _fileStorageMock
                .Setup(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedUrl);

            using var memoryStream = new MemoryStream();
            var command = new UpdateDoctorPhotoCommand(
                memoryStream,
                "avatar.jpg",
                "image/jpeg"
            );

            await _handler.Handle(command, CancellationToken.None);

            var updatedDoctor = await _context.Doctors.FirstAsync();
            updatedDoctor.ImageUrl.Should().Be(expectedUrl);

            _fileStorageMock.Verify(x => x.UploadFileAsync(
                It.IsAny<Stream>(),
                "avatar.jpg",
                "image/jpeg"),
                Times.Once);
        }
    }
}