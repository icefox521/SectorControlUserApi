using Dapper;
using Npgsql;
using SectorControlApi.Helpers.Dapper;
using SectorControlApi.Data.Users;

namespace SectorControlApi.Services
{
    public class UserService : IUserService
    {
        IConfiguration _configuration;
        ILogService _logger;
        IPasswordService _passwordService;
        ILogService _logService;

        public UserService(IConfiguration configuration, ILogService logger, IPasswordService passwordService, ILogService logService)
        {
            _configuration = configuration;
            _logger = logger;
            _passwordService = passwordService;
            _logService = logService;
        }

        /// <summary>
        /// Creates new user in DB.
        /// </summary>
        public async Task CreateUserInDatabaseAsync(string username, string email, string salt, string passwordHash)
        {
            SqlMapper.SetTypeMap(typeof(User), new ColumnAttributeTypeMapper<User>());

            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var insertStatement = @"INSERT INTO users (username, email, salt, password_hash, registration_date) VALUES (@username, @email, @salt, @passwordHash, @registrationDate)";
                await connection.ExecuteAsync(insertStatement, new { username, email, salt, passwordHash, registrationDate = DateTime.UtcNow });
            }

        }
        /// <summary>
        /// Retrieves user id, salt and password hash from DB.
        /// </summary>
        public async Task<User> RetrieveUserLoginDetailAsync(string email)
        {
            User user = null;
            try
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    var selectStatement = @"SELECT id, salt, password_hash FROM users WHERE email = @email";
                    var result = await connection.QueryAsync<User>(selectStatement, new { email });
                    if (result.Count() == 1)
                    {
                        user = result.First();
                    }
                    else
                    {
                        if (result.Count() > 0)
                        {
                            _logger.Error("Error, more users with same email retrieved.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error. " + ex);
            }

            return user;
        }

        /// <summary>
        /// Authenticate user.
        /// </summary>
        /// <returns>
        /// Guid presenting access token for user.
        /// </returns>
        public async Task<Guid?> AuthenticateAsync(string email, string password)
        {
            bool isAuthenticated = false;
            Guid? token = null;

            User user = await RetrieveUserLoginDetailAsync(email);

            if (user == null)
            {
                _logService.Error($"Error, user with email {email} was not found");
            }

            if (!string.IsNullOrEmpty(user.HashSalt) && !string.IsNullOrEmpty(user.PasswordHash))
            {
                byte[] bytesSalt = Convert.FromBase64String(user.HashSalt);
                byte[] bytesHash = Convert.FromBase64String(user.PasswordHash);
                isAuthenticated = await _passwordService.VerifyHashAsync(password, bytesSalt, bytesHash);
            }
            else
            {
                _logService.Error($"Error, user with email {email} has no salt or password");
            }

            if (isAuthenticated)
            {
                token = Guid.NewGuid();
                await SaveTokenAsync(token, user.Id);
            }

            return token;
        }

        /// <summary>
        /// Verify user's access token. 
        /// </summary>
        /// <returns>
        /// If success user id otherwise null.
        /// </returns>
        public async Task<int?> VerifyTokenAsync(string token)
        {
            int? returnVal = null;
            if (token != null)
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    var selectStatement = @"SELECT id FROM public.users	WHERE auth_token = @token AND auth_token_expiration >= @dateTimeNow;";
                    var result = await connection.QueryAsync<int>(selectStatement, new { token, dateTimeNow = DateTime.UtcNow });
                    if (result.Count() == 1)
                    {
                        returnVal = result.First();
                    }
                    else
                    {
                        if (result.Count() > 1)
                        {
                            List<int> list = new List<int>();
                            foreach(int res in result)
                            {
                                list.Add(res); 
                            }
                            _logger.Error("Error, retrieved more records with same token. Retrieved records: " +  string.Join(",", list));
                        }
                    }
                }
            }
            return returnVal;
        }
        /// <summary>
        /// Saves access token to DB and sets expiration date in one day.
        /// </summary>
        private async Task SaveTokenAsync(Guid? token, int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    var updateStatement = @"UPDATE public.users	SET auth_token = @token, auth_token_expiration = @expiration, last_logged_in = @lastLoggedIn WHERE id = @id;";
                    var result = await connection.ExecuteAsync(updateStatement, new { token, expiration = DateTime.UtcNow.AddHours(24), id, lastLoggedIn = DateTime.UtcNow });
                    if (result != 1)
                    {
                        _logger.Error("Error, updating token failed. Couldnt update token for user " + id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error. " + ex);
            }
        }
    }
}
