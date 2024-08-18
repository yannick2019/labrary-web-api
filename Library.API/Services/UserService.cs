using Library.API.Data;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Services
{
    public class UserService : IUserService
    {
        private readonly LibraryContext _context;
        private readonly UserManager<User> _userManager;

        public UserService(LibraryContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email
            });
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<User>> GetPaginatedUsersAsync(UserParameters userParameters)
        {
            IQueryable<User> users = _context.Users;

            // Appliquer le tri
            switch (userParameters.SortBy.ToLower())
            {
                case "email":
                    users = userParameters.SortDescending
                        ? users.OrderByDescending(u => u.Email)
                        : users.OrderBy(u => u.Email);
                    break;
                default: // Par défaut, trier par nom d'utilisateur
                    users = userParameters.SortDescending
                        ? users.OrderByDescending(u => u.UserName)
                        : users.OrderBy(u => u.UserName);
                    break;
            }

            var count = await users.CountAsync();
            var items = await users.Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
                                   .Take(userParameters.PageSize)
                                   .ToListAsync();

            return new PaginatedList<User>(items, count, userParameters.PageNumber, userParameters.PageSize);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}