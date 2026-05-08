using Rupestre.Application.DTOs;

namespace Rupestre.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync();
}
