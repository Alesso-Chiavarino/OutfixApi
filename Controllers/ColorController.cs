using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using OutfixApi.Repositories;

namespace OutfixApi.Controllers;

[Route("api/colors")]
[ApiController]
public class ColorController : ControllerBase
{
    private readonly IColorCollection _colors;

    public ColorController()
    {
        _colors = new ColorCollection();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _colors.GetAllColors();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _colors.GetColorById(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Color color)
    {
        if (color == null || string.IsNullOrEmpty(color.Name))
            return BadRequest("Invalid color.");

        await _colors.AddColor(color);
        return CreatedAtAction(nameof(GetById), new { id = color.Id }, color);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Color color)
    {
        var existing = await _colors.GetColorById(id);
        if (existing == null)
            return NotFound();

        color.Id = id;
        await _colors.UpdateColor(color);
        return Ok(color);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _colors.GetColorById(id);
        if (existing == null)
            return NotFound();

        await _colors.DeleteColor(id);
        return NoContent();
    }
}