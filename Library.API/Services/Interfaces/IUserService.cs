using Library.API.Models;

namespace Library.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<PaginatedList<User>> GetPaginatedUsersAsync(UserParameters userParameters);
    }
}