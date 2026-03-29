using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

public class SliderBannerService : ISliderBannerService
{
    private readonly GabriniCosmeticsContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<SliderBannerService> _logger;

    public SliderBannerService(GabriniCosmeticsContext context, IWebHostEnvironment environment, ILogger<SliderBannerService> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    public async Task<List<SliderBannerDTO>> GetSliderBanners()
    {
        try
        {
            return await _context.SliderBanners
                .AsNoTracking()
                .Select(sb => new SliderBannerDTO
                {
                    Id = sb.Id,
                    ImageUrl = sb.ImageUrl
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting slider banners");
            throw;
        }
    }

    public async Task<SliderBannerDTO> CreateSliderBanner(CreateSliderBannerDTO createSliderBannerDto)
    {
        try
        {
            var sliderBanner = new SliderBanner();

            if (createSliderBannerDto.ImageUpload != null)
            {
                var fileName = await SaveImageAsync(createSliderBannerDto.ImageUpload);
                sliderBanner.ImageUrl = fileName;
            }

            _context.SliderBanners.Add(sliderBanner);
            await _context.SaveChangesAsync();

            return new SliderBannerDTO
            {
                Id = sliderBanner.Id,
                ImageUrl = sliderBanner.ImageUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating slider banner");
            throw;
        }
    }

    public async Task<SliderBannerDTO> GetSliderBannerById(int id)
    {
        var sliderBanner = await _context.SliderBanners.FindAsync(id);
        if (sliderBanner == null)
        {
            return null;
        }

        return new SliderBannerDTO
        {
            Id = sliderBanner.Id,
            ImageUrl = sliderBanner.ImageUrl
        };
    }

    public async Task UpdateSliderBanner(UpdateSliderBannerDTO updateSliderBannerDto)
    {
        var sliderBanner = await _context.SliderBanners.FindAsync(updateSliderBannerDto.Id);
        if (sliderBanner == null)
        {
            throw new KeyNotFoundException($"SliderBanner with id {updateSliderBannerDto.Id} not found.");
        }

        if (updateSliderBannerDto.ImageUpload != null)
        {
            var fileName = await SaveImageAsync(updateSliderBannerDto.ImageUpload);
            sliderBanner.ImageUrl = fileName;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteSliderBanner(int id)
    {
        try
        {
            var sliderBanner = await _context.SliderBanners.FirstOrDefaultAsync(sb => sb.Id == id);

            if (sliderBanner == null)
                return false;

            _context.SliderBanners.Remove(sliderBanner);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting slider banner with id {id}");
            throw;
        }
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
