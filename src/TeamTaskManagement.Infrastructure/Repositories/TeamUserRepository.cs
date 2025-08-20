using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Domain.Entities.Enums;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TeamTaskManagement.Infrastructure.Repositories
{
    public class TeamUserRepository : ITeamUserRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TeamUser> AddUserToTeamAsync(Guid userId, Guid teamId, TeamRole role = TeamRole.Member)
        {
            var teamUser = new TeamUser
            {
                UserId = userId,
                TeamId = teamId,
                Role = role
            };

            _context.TeamUsers.Add(teamUser);
            await _context.SaveChangesAsync();
            return teamUser;
        }

        public async Task<TeamUser?> GetTeamUserAsync(Guid userId, Guid teamId)
        {
            return await _context.TeamUsers
                .Include(tu => tu.User)
                .Include(tu => tu.Team)
                .FirstOrDefaultAsync(tu => tu.UserId == userId && tu.TeamId == teamId);
        }

        public async Task<bool> IsUserInTeamAsync(Guid userId, Guid teamId)
        {
            return await _context.TeamUsers
                .AnyAsync(tu => tu.UserId == userId && tu.TeamId == teamId);
        }
    }
}
