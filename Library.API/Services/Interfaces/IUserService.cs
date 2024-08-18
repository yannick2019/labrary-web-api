using Library.API.Models;

namespace Library.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User> AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
        Task<PaginatedList<User>> GetPaginatedUsersAsync(UserParameters userParameters);
    }
}