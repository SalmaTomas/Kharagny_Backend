using Kharagny_Backend.Data;
using Kharagny_Backend.DTOs;
using Kharagny_Backend.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kharagny_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferencesSurveyController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public PreferencesSurveyController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var questions = await _context.Question.Include(q => q.Choices).ToListAsync();

            var questionDtos = questions.Select(q => new QuestionsDto
            {
                Id = q.Id,
                Text = q.Text,
                Choices = q.Choices.Select(c => new ChoiceDto
                {
                    Id = c.Id,
                    Text = c.Text
                }).ToList()
            }).ToList();

            return Ok(questions);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitResponses([FromBody] List<ResponseDto> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                return BadRequest("No responses provided");
            }
            int userId = 2;
            var userResponses = new List<Response>();
            foreach (var response in responses)
            {
                var selectedChoicesString = string.Join(",", response.SelectedChoiceIds);
                var userResponse = new Response
                {
                    UserId = userId,
                    QuestionId = response.QuestionId,
                    SelectedChoices = selectedChoicesString
                };
                userResponses.Add(userResponse);
            }
            await _context.Responses.AddRangeAsync(userResponses);
            await _context.SaveChangesAsync();
            return Ok("Survey Submitted Successfully");
        }
    }
}
