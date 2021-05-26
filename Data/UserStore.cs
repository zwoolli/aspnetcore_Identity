using Microsoft.AspNetCore.Identity;
using WebApp.Models;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Dapper;

namespace WebApp.Data
{
    public class UserStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserPhoneNumberStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private readonly NpgsqlConnection _connection;

        public UserStore(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"INSERT INTO applicationuser (username, normalizedusername, email, normalizedemail, emailconfirmed, 
                                                            passwordhash, phonenumber, phonenumberconfirmed, twofactorenabled) 
                            VALUES (@{nameof(ApplicationUser.UserName)}, @{nameof(ApplicationUser.NormalizedUserName)}, @{nameof(ApplicationUser.Email)},
                                    @{nameof(ApplicationUser.NormalizedEmail)}, @{nameof(ApplicationUser.EmailConfirmed)}, @{nameof(ApplicationUser.PasswordHash)},
                                    @{nameof(ApplicationUser.PhoneNumber)}, @{nameof(ApplicationUser.PhoneNumberConfirmed)}, @{nameof(ApplicationUser.TwoFactorEnabled)}) 
                            RETURNING id";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                user.Id = await _connection.QuerySingleAsync<int>(sql, user);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"DELETE 
                            FROM applicationuser 
                            WHERE id = @{nameof(ApplicationUser.Id)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                await _connection.ExecuteAsync(sql, user);
            }            

            return IdentityResult.Success;
        }
      
        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"SELECT * 
                            FROM applicationuser 
                            WHERE id = @{nameof(userId)}::integer";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(sql, new {userId});
            }
        }
      
        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"SELECT * 
                            FROM applicationuser 
                            WHERE normalizedusername = @{nameof(normalizedUserName)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(sql, new {normalizedUserName});
            }
        }
       
        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }
      
        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {   
            return Task.FromResult(user.Id.ToString());
        }
       
        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }
       
        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"UPDATE applicationuser 
                            SET username = @{nameof(ApplicationUser.UserName)},
                                normalizedusername = @{nameof(ApplicationUser.NormalizedUserName)},
                                email = @{nameof(ApplicationUser.Email)},
                                normalizedemail = @{nameof(ApplicationUser.NormalizedEmail)},
                                emailconfirmed = @{nameof(ApplicationUser.EmailConfirmed)},
                                passwordhash = @{nameof(ApplicationUser.PasswordHash)},
                                phonenumber = @{nameof(ApplicationUser.PhoneNumber)},
                                phonenumberconfirmed = @{nameof(ApplicationUser.PhoneNumberConfirmed)},
                                twofactorenabled = @{nameof(ApplicationUser.TwoFactorEnabled)} 
                            WHERE id = @{nameof(ApplicationUser.Id)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                await _connection.ExecuteAsync(sql, user);
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string sql = $@"SELECT * 
                            FROM applicationuser 
                            WHERE normalizedemail = @{nameof(ApplicationUser.NormalizedEmail)}";

            using (_connection)
            {
                await _connection.OpenAsync(cancellationToken);
                return await _connection.QuerySingleOrDefaultAsync<ApplicationUser>(sql, new { normalizedEmail });
            }
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }
        
        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }
        
        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }
        
        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public void Dispose() {}
    }
}