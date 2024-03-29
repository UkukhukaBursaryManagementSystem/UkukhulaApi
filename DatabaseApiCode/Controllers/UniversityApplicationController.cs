#pragma warning disable CS8601
#pragma warning disable CS1591
#pragma warning disable CS8618

namespace DatabaseApiCode.Controllers
{    // Controller for Inserting Values in the University Applications Table 
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityApplicationController : ControllerBase
    {
        private readonly string _connectionString;

        public UniversityApplicationController(IConfiguration configuration)

        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] UniversityApplicationModel universityApplicationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"
                        INSERT INTO UniversityApplication (ApplicationStatusID, AmountRequested, UniversityID)
                        VALUES (@ApplicationStatusID, @AmountRequested, @UniversityID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationStatusID", universityApplicationModel.ApplicationStatusID);
                        command.Parameters.AddWithValue("@AmountRequested", universityApplicationModel.AmountRequested);
                        command.Parameters.AddWithValue("@UniversityID", universityApplicationModel.UniversityID);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("University Application added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetAllUniversityApplications()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT ApplicationID, ApplicationStatusID, AmountRequested, UniversityID FROM UniversityApplication";
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var universityApplications = new List<UniversityApplicationModel>();
                        while (await reader.ReadAsync())
                        {
                            var universityApplication = new UniversityApplicationModel
                            {
                                ApplicationID = reader.GetInt32(0),
                                ApplicationStatusID = reader.GetInt32(1),
                                // Changed model to Decimal
                                AmountRequested = reader.GetDecimal(2),
                                UniversityID = reader.GetInt32(3)
                            };
                            universityApplications.Add(universityApplication);
                        }
                        return Ok(universityApplications);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUniversityApplicationStatus([FromBody] UniversityApplicationModel universityApplicationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"
                        UPDATE UniversityApplication SET ApplicationStatusID = @ApplicationStatusID WHERE ApplicationID = @ApplicationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationStatusID", universityApplicationModel.ApplicationStatusID);
                        command.Parameters.AddWithValue("@ApplicationID", universityApplicationModel.ApplicationID);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("University Application updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }


        // Get Application by ID
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetUniversityApplicationById(int applicationId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT ApplicationID, ApplicationStatusID, AmountRequested, UniversityID FROM UniversityApplication WHERE ApplicationID = @ApplicationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", applicationId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var universityApplication = new UniversityApplicationModel
                                {
                                    ApplicationID = reader.GetInt32(0),
                                    ApplicationStatusID = reader.GetInt32(1),
                                    // Changed model to Decimal
                                    AmountRequested = reader.GetDecimal(2),
                                    UniversityID = reader.GetInt32(3)
                                };
                                return Ok(universityApplication);
                            }
                            else
                            {
                                return NotFound(); // Application with the specified ID not found
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
