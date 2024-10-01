using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Business.Services;
using MyBlog.Entities.DTOs;
using Serilog;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;
    private readonly ILogger<GalleryController> _logger;

    public GalleryController(IGalleryService galleryService, ILogger<GalleryController> logger)
    {
        _galleryService = galleryService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("GalleryController.GetAll method called");
        var galleries = _galleryService.GetAll();
        return Ok(galleries);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        _logger.LogInformation("GalleryController.GetById method called with id: {Id}", id);
        var gallery = _galleryService.GetById(id);
        if (gallery == null)
        {
            _logger.LogWarning("Gallery with id {Id} not found", id);
            return NotFound();
        }
        return Ok(gallery);
    }

    [HttpPost]
    public IActionResult Add([FromBody] GalleryDTO galleryDto)
    {
        _logger.LogInformation("GalleryController.Add method called with data: {@GalleryDTO}", galleryDto);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for GalleryDTO: {@GalleryDTO}", galleryDto);
            return BadRequest(ModelState);
        }

        var newGallery = _galleryService.Add(galleryDto);
        return CreatedAtAction(nameof(GetById), new { id = newGallery.Id }, newGallery);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] GalleryDTO galleryDto)
    {
        _logger.LogInformation("GalleryController.Update method called with id: {Id} and data: {@GalleryDTO}", id, galleryDto);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for GalleryDTO: {@GalleryDTO}", galleryDto);
            return BadRequest(ModelState);
        }

        galleryDto.Id = id;
        var updatedGallery = _galleryService.Update(galleryDto);
        return Ok(updatedGallery);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation("GalleryController.Delete method called with id: {Id}", id);
        _galleryService.Delete(id);
        return NoContent();
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? title, [FromQuery] string? type, [FromQuery] string? description)
    {
        _logger.LogInformation("GalleryController.Search method called with title: {Title}, type: {Type}, description: {Description}", title, type, description);
        var result = _galleryService.Search(title, type, description);
        return Ok(result);
    }
    
}