using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GabriniCosmetics.Areas.Admin.Models.DTOs;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface ISocialMediaService
    {
        Task Add(SocialMediaForm socialMedias);
        Task Edit(SocialMediaForm socialMedias);
        Task Delete(int id);
        Task<List<SocialMedia>> GetAll();
        Task<SocialMediaForm> Get(int id);
    }
}