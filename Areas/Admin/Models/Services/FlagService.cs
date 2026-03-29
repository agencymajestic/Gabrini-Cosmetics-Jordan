using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FlagService : IFlagService
{
    private readonly GabriniCosmeticsContext _context;
    private readonly ILogger<FlagService> _logger;

    public FlagService(GabriniCosmeticsContext context, ILogger<FlagService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Flag>> GetAllFlagsAsync()
    {
        try
        {
            return await _context.Flags.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving flags");
            throw;
        }
    }
    public async Task<Flag> GetFlagByNameAsync(string name)
    {
        try
        {
            return await _context.Flags
                .FirstOrDefaultAsync(f => f.Name == name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving flag by name");
            throw;
        }
    }
}
