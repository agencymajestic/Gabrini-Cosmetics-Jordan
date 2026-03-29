using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

public class SliderAdService : ISliderAdService
{
    private readonly GabriniCosmeticsContext _context;
    private readonly IWebHostEnvironment _environment;

    public SliderAdService(GabriniCosmeticsContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<List<SliderAdDTO>> GetAllSliderAds()
    {
        try
        {
            return await _context.SliderAds
                .AsNoTracking()
                .Select(sa => new SliderAdDTO
                {
                    Id = sa.Id,
                    ImageUrl = sa.ImageUrl,
                    Link = sa.Link,
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            // Log the exception
            throw;
        }
    }

    public async Task<SliderAdDTO> GetSliderAdById(int id)
    {
        var sliderAd = await _context.SliderAds.FindAsync(id);
        if (sliderAd == null)
        {
            return null;
        }

        return new SliderAdDTO
        {
            Id = sliderAd.Id,
            ImageUrl = sliderAd.ImageUrl,
            Link = sliderAd.Link
        };
    }

    public async Task UpdateSliderAd(UpdateSliderAdDTO updateSliderAdDto)
    {
        var sliderAd = await _context.SliderAds.FindAsync(updateSliderAdDto.Id);
        if (sliderAd == null)
        {
            throw new KeyNotFoundException($"SliderAd with id {updateSliderAdDto.Id} not found.");
        }

        if (updateSliderAdDto.ImageUpload != null)
        {
            var fileName = await SaveImageAsync(updateSliderAdDto.ImageUpload);
            sliderAd.ImageUrl = fileName;
        }
        sliderAd.Link = updateSliderAdDto.Link;
        await _context.SaveChangesAsync();
    }

    public async Task CreateSliderAd(CreateSliderAdDTO createSliderAdDto)
    {
        var sliderAd = new SliderAd
        {
            ImageUrl = createSliderAdDto.ImageUpload != null
                ? await SaveImageAsync(createSliderAdDto.ImageUpload)
                : null,
            Link = createSliderAdDto.Link
        };

        _context.SliderAds.Add(sliderAd);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteSliderAd(int id)
    {
        var sliderAd = await _context.SliderAds.FindAsync(id);
        if (sliderAd == null)
        {
            return false;
        }

        _context.SliderAds.Remove(sliderAd);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> SaveImageAsync(IFormFile imageUpload)
    {
        if (imageUpload == null || imageUpload.Length == 0)
        {
            throw new ArgumentException("Invalid image upload");
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUpload.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageUpload.CopyToAsync(fileStream);
        }

        return fileName;
    }
}
