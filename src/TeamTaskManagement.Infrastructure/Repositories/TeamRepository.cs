using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Infrastructure.Data;

namespace TeamTaskManagement.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Team?> GetByIdAsync(Guid id)
        {
            return await _context.Teams
                .Include(t => t.TeamUsers)
                .ThenInclude(tu => tu.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Team> CreateAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<IEnumerable<Team>> GetUserTeamsAsync(Guid userId)
        {
            return await _context.Teams
                .Where(t => t.TeamUsers.Any(tu => tu.UserId == userId))
                .ToListAsync();
        }

        public async Task<bool> IsUserInTeamAsync(Guid userId, Guid teamId)
        {
            return await _context.TeamUsers
                .AnyAsync(tu => tu.UserId == userId && tu.TeamId == teamId);
        }
    }
}
