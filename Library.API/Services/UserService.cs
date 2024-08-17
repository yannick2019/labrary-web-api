using Library.API.Data;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Services
{
    public class UserService : IUserService
    {
        private readonly LibraryContext _context;

        public UserService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
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
                        ? users.OrderByDescending(u => u.Username)
                        : users.OrderBy(u => u.Username);
                    break;
            }

            var count = await users.CountAsync();
            var items = await users.Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
                                   .Take(userParameters.PageSize)
                                   .ToListAsync();

            return new PaginatedList<User>(items, count, userParameters.PageNumber, userParameters.PageSize);
        }

        public async Task DeleteUserAsync(int id)
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