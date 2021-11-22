using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;
using TelephoneBookAssignment.Services.RabbitMQ;
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


        public ReportsController(IMongoTelephoneBookDbContext context, RabbitMQPublisher rabbitMqPublisher)
        {
            _context = context;
            _rabbitMqPublisher = rabbitMqPublisher;
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
           // await _myHubContext.Clients.User(userFile.UserId).SendAsync("CompletedFile");

            return Ok();
        }

        [HttpPost("createExcel")]
        public async Task<IActionResult> CreateExcel()
        {
            //var user = await _userManager.FindByNameAsync(User.Identity.Name); // From cookie

            var fileName = $"report-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            ReportFile userFile = new()
            {
                //UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating,
                RequestedDate = DateTime.Now
            };

            await _dbCollection.InsertOneAsync(userFile);

            // RabbitMQ Publish Part
            _rabbitMqPublisher.Publish(new CreateReportMessage() { ObjectId = userFile.ObjectId.ToString() });

            // Transfer a data from one request to another request, stores data in Cookie
            //TempData["CreatingExcelStarted"] = true; // Cookie

            return Ok(true);
        }

        [HttpGet]
        public async Task<IActionResult> Files()
        {
            //var user = await _userManager.FindByNameAsync(User.Identity.Name); // From cookie
            var files = await _dbCollection.Find(x => true).ToListAsync();

            return Ok(files);
        }
    }
}