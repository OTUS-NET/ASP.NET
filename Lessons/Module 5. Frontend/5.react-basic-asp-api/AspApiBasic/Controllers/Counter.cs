using Microsoft.AspNetCore.Mvc;

namespace AspApiBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CounterController : ControllerBase
{
    // Переменная для хранения значения счетчика (в памяти)
    private static int _counter = 0;

    // GET: api/counter
    [HttpGet]
    public IActionResult GetCounter()
    {
        // Возвращаем текущее значение счетчика
        return Ok(_counter);
    }

    // POST: api/counter/increment
    [HttpPost("increment")]
    public IActionResult IncrementCounter()
    {
        // Увеличиваем счетчик на 1
        _counter++;
        return Ok(_counter);
    }

    // POST: api/counter/decrement
    [HttpPost("decrement")]
    public IActionResult DecrementCounter()
    {
        // Уменьшаем счетчик на 1
        _counter--;
        return Ok(_counter);
    }

    // POST: api/counter/reset
    [HttpPost("reset")]
    public IActionResult ResetCounter()
    {
        // Сбрасываем счетчик до 0
        _counter = 0;
        return Ok(_counter);
    }
}