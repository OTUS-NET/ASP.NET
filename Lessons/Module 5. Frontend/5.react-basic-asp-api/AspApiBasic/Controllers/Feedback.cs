using AspApiBasic.Extensions;
using Microsoft.AspNetCore.Mvc;
using AspApiBasic.Model;
using Microsoft.AspNetCore.Cors;

namespace AspApiBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    // Пример хранения обратной связи (в реальном приложении используйте базу данных)
    private static readonly List<Feedback> Feedbacks = new();

    // POST: api/feedback
    [HttpPost]
    [EnableCors(policyName: ApiCorsPolicies.AllowSpecificRoute)]
    public IActionResult SubmitFeedback([FromBody] Feedback feedback)
    {
        if (feedback == null || string.IsNullOrEmpty(feedback.Message))
        {
            return BadRequest("Сообщение обязательно для заполнения.");
        }

        Feedbacks.Add(feedback);
        return Ok("Спасибо за ваш отзыв!");
    }

    // GET: api/feedback
    [HttpGet]
    public IActionResult GetFeedbacks()
    {
        return Ok(Feedbacks);
    }
}