using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class SocialMediaService : ISocialMediaService
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly IWebHostEnvironment _environment;
        public SocialMediaService(GabriniCosmeticsContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<SocialMedia>> GetAll()
        {
            return await _context.SocialMedias.ToListAsync();
        }

        public async Task<SocialMediaForm> Get(int id)
        {
            var socialMedia = await _context.SocialMedias.SingleOrDefaultAsync(s => s.Id == id);
            if(socialMedia is null)
            {
                throw new Exception("Social media not found");
            }

            return new SocialMediaForm
            {
                Id = socialMedia.Id,
                Url = socialMedia.Url,
                ExistingImagePath = socialMedia.Image
            };
        }

        public async Task Add(SocialMediaForm socialMedia)
        {
                try
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(socialMedia.Image.FileName)}";
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await socialMedia.Image.CopyToAsync(fileStream);
                    }

                    var newSocialMedia = new SocialMedia
                    {
                        Url = socialMedia.Url,
                        Image = fileName

                    };

                    await _context.SocialMedias.AddAsync(newSocialMedia);

                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding social media", ex);
                }


            await _context.SaveChangesAsync();
        }

        public async Task Edit(SocialMediaForm socialMedia)
        {
            if(socialMedia.Id is null)
            {
                throw new Exception("Id is required");
            }

            var existingSocialMedia = await _context.SocialMedias.FindAsync(socialMedia.Id) 
            ?? throw new KeyNotFoundException($"Social media with id {socialMedia.Id} not found.");

            if (socialMedia.Image is not null)
            {
                try
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(socialMedia.Image.FileName)}";
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await socialMedia.Image.CopyToAsync(fileStream);

                    existingSocialMedia.Image = fileName;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding social media", ex);
                }
            }

            existingSocialMedia.Url = socialMedia.Url;
            _context.SocialMedias.Update(existingSocialMedia);
            
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var socialMedia = await _context.SocialMedias.FindAsync(id);
            if(socialMedia is null)
            {
                return;
            }
            _context.SocialMedias.Remove(socialMedia);
            await _context.SaveChangesAsync();
        }
    }
}