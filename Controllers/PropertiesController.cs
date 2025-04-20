using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.Model;


[Route("api/[controller]")]
[ApiController]
public class PropertiesController : ControllerBase
{
    private readonly HomeDB _context;
    private readonly IWebHostEnvironment _env;

    public PropertiesController(HomeDB context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: api/Properties
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
    {
        return await _context.Properties.ToListAsync();
    }

    // GET: api/Properties/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Property>> GetProperty(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null) return NotFound();
        return property;
    }

    // POST: api/Properties
    //[HttpPost]
    //public async Task<ActionResult<Property>> PostProperty(Property property)
    //{
    //    _context.Properties.Add(property);
    //    await _context.SaveChangesAsync();
    //    return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
    //}


    [HttpPost("create")]
    public async Task<IActionResult> CreateProperty([FromForm] PropertyFormDto dto)
    {
        if (dto.ImageFile == null || dto.ImageFile.Length == 0)
            return BadRequest("Image file is required.");

        // Save image to wwwroot/images
        var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.ImageFile.CopyToAsync(stream);
        }

        var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";

        // Save property with image URL
        var property = new Property
        {
            Title = dto.Title,
            Address = dto.Address,
            listingType=dto.listingType,
            RentAmount = dto.RentAmount,
            Bedrooms = dto.Bedrooms,
            IsFurnished = dto.IsFurnished,
            PropertyType = dto.PropertyType,
            AreaInSqFt = dto.AreaInSqFt,
            AvailableFrom = dto.AvailableFrom,
            OwnerName = dto.OwnerName,
            ContactNumber = dto.ContactNumber,
            ImageUrls = imageUrl
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        return Ok(property);
    }


    // PUT: api/Properties/5
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutProperty(int id, Property property)
    //{
    //    if (id != property.Id) return BadRequest();

    //    _context.Entry(property).State = EntityState.Modified;
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProperty(int id, [FromForm] PropertyUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("Property ID mismatch.");

        var property = await _context.Properties.FindAsync(id);
        if (property == null) return NotFound("Property not found.");

        // Update property details
        property.Title = dto.Title;
        property.Address = dto.Address;
        property.RentAmount = dto.RentAmount;
        property.Bedrooms = dto.Bedrooms;
        property.IsFurnished = dto.IsFurnished;
        property.PropertyType = dto.PropertyType;
        property.AreaInSqFt = dto.AreaInSqFt;
        property.AvailableFrom = dto.AvailableFrom;
        property.OwnerName = dto.OwnerName;
        property.ContactNumber = dto.ContactNumber;
        property.listingType = dto.listingType;

        // Handle image upload if new image is provided
        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";
            property.ImageUrls = imageUrl;
        }

        try
        {
            _context.Entry(property).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PropertyExists(id))
            {
                return NotFound("Property not found during update.");
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // Success without content
    }


    // DELETE: api/Properties/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null) return NotFound();

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PropertyExists(int id)
    {
        return _context.Properties.Any(e => e.Id == id);
    }
}
