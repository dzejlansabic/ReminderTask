using ReminderTask.Application.DTOs;

namespace ReminderTask.Application.Services
{
    public interface IReminderService
    {
        Task<IReadOnlyList<ReminderResponse>> GetAllAsync(CancellationToken ct = default);
        Task<ReminderResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ReminderResponse> CreateAsync(ReminderCreateRequest request, CancellationToken ct = default);
        Task<ReminderResponse> UpdateAsync(Guid id, ReminderUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
