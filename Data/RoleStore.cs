using Microsoft.AspNetCore.Identity;
using WebApp.Models;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using System.Data.Common;

namespace WebApp.Data
{
    public class RoleStore : IRoleStore<ApplicationRole>
    {
        private readonly DbConnection _connection;

        public RoleStore(DbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"INSERT INTO applicationrole (name, normalizedname) 
                            VALUES (@{nameof(ApplicationRole.Name)}, @{nameof(ApplicationRole.NormalizedName)}) 
                            RETURNING id";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                role.Id = await _connection.QuerySingleAsync<int>(sql, role);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"DELETE FROM applicationrole 
                            WHERE id = @{nameof(ApplicationRole.Id)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                await _connection.ExecuteAsync(sql, role);
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"SELECT * 
                            FROM applicationrole 
                            WHERE id = @{nameof(roleId)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                return await _connection.QuerySingleOrDefaultAsync<ApplicationRole>(sql, new { roleId });
            }
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"SELECT * 
                            FROM applicationrole 
                            WHERE normalizedname = @{nameof(normalizedRoleName)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                return await _connection.QuerySingleOrDefaultAsync<ApplicationRole>(sql, new { normalizedRoleName });
            }     
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"UPDATE applicationrole 
                            SET name = @{nameof(ApplicationRole.Name)}, normalizedname = @{nameof(ApplicationRole.NormalizedName)} 
                            WHERE id = @{nameof(ApplicationRole.Id)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                await _connection.ExecuteAsync(sql, role);
            }

            return IdentityResult.Success;
        }

        public void Dispose(){}
    }
}