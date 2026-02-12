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
        
    }
}