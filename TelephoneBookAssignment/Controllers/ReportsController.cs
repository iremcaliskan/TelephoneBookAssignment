using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TelephoneBookAssignment.Services.RabbitMQ;
using TelephoneBookAssignment.Services.SignalR.Hubs;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;
using TelephoneBookAssignment.Shared.Entities;
using TelephoneBookAssignment.Shared.Services.RabbitMQ;

namespace TelephoneBookAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMongoTelephoneBookDbContext _context;
        private readonly IMongoCollection<ReportFile> _dbCollection;
        private readonly RabbitMQPublisher _rabbitMqPublisher;
        private readonly IHubContext<ExcelHub> _hubContext;

        public ReportsController(IMongoTelephoneBookDbContext context, RabbitMQPublisher rabbitMqPublisher, IHubContext<ExcelHub> hubContext)
        {
            _context = context;
            _rabbitMqPublisher = rabbitMqPublisher;
            _hubContext = hubContext;
            _dbCollection = _context.GetCollection<ReportFile>(nameof(ReportFile));
        }

        [HttpPost("uploadExcel")]
        public async Task<IActionResult> Upload(IFormFile file, string fileId)
        {
            if (file is not { Length: > 0 })
            {
                return BadRequest();
            }

            var userFile = await _context.GetCollection<ReportFile>(nameof(ReportFile)).Find(x => x.ObjectId == new ObjectId(fileId)).FirstOrDefaultAsync();

            var filePath = userFile.FileName + Path.GetExtension(file.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);

            await using FileStream stream = new(path, FileMode.Create);

            await file.CopyToAsync(stream); // Copy the content of file to stream

            userFile.CompletedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;

            await _dbCollection.FindOneAndReplaceAsync(x => x.ObjectId == new ObjectId(fileId), userFile);

            // SignalR - Real Time Notification
            await _hubContext.Clients.User(userFile.UserId).SendAsync("ExcelCreationCompleted");

            return Ok();
        }

        [HttpPost("createExcel")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateExcel()
        {
            if (User.Identity is {IsAuthenticated: false})
            {
                throw new AuthenticationException("Please login.");
            }

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var fileName = $"report-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            ReportFile userFile = new()
            {
                UserId = userId,
                FileName = fileName,
                FileStatus = FileStatus.Creating,
                RequestedDate = DateTime.Now
            };

            await _dbCollection.InsertOneAsync(userFile);

            // RabbitMQ Publish Part
            _rabbitMqPublisher.Publish(new CreateReportMessage() { ObjectId = userFile.ObjectId.ToString() });

            // SignalR - Real Time Notification
            if (userFile.UserId != null)
            {
                await _hubContext.Clients.User(userFile.UserId).SendAsync("ExcelCreationStarted");
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Files()
        {
            if (User.Identity is { IsAuthenticated: false })
            {
                throw new AuthenticationException("Please login.");
            }

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var files = await _dbCollection.Find(x => x.UserId == userId).ToListAsync();

            return Ok(files);
        }
    }
}