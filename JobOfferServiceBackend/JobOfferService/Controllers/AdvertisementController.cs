using JobOfferService.Database.Context;
using JobOfferService.Database.Models;
using JobOfferService.Models.Dto;
using JobOfferService.Models.Dto.JobAdvertisementDetailsDto;
using JobOfferService.Models.Dto.JobAdvertisementDto;
using JobOfferService.Utilities.Authentication;
using JobOfferService.Utilities.Constants;
using JobOfferService.Utilities.PasswordClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobOfferService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly IUserHandler _userHandler;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;

        public AdvertisementController(ApplicationDbContext context, IUserAuthenticator userAuthenticator, 
            IHttpContextAccessor contextAccessor, ILogger<AdvertisementController> logger, IUserHandler userhandler)
        {
            _context = context;
            _userAuthenticator = userAuthenticator;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _userHandler = userhandler;
        }

        /// <summary>
        /// Returns the list of Advertisement
        /// If the user is admin -> returns only his advertisements
        /// If the user is employee -> returns all the advertisements
        /// Supports pagination
        /// </summary>
        [HttpGet("{page}")]
        [Route("AdvertisementsList")]
        public async Task<IActionResult> AdvertisementsList([FromQuery] int page)
        {
            try
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken)) return new UnauthorizedResult();

                var user = _userAuthenticator.GetUserByToken(accessToken);
                if (user == null) return BadRequest();

                var dbAdvertisements = user.AccountType == Database.Models.AccountTypes.USER
                    ? _context.JobAdvertisements.ToList()
                    : _context.JobAdvertisements.Where(x => x.EmployerId == user.Id).ToList();

                var advertisementsList = dbAdvertisements.Skip(page == 1 ? 0 : (page - 1) * Constants.RecordsPerPage).Take(Constants.RecordsPerPage).ToList();
                var advertisementModels = advertisementsList.Select(x => new JobAdvertisementListDto(x)).ToList();

                return Ok(advertisementModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured when processing the AdvertisementsList request" + ex.StackTrace + "\n" + ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Returns the Avertisement details
        /// </summary>
        [HttpGet("{id}")]
        [Route("AdvertisementDetails")]
        public async Task<IActionResult> AdvertisementDetails([FromQuery] int id)
        {
            try
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken)) return new UnauthorizedResult();

                var user = _userAuthenticator.GetUserByToken(accessToken);
                if (user == null) return BadRequest();

                var accountType = user.AccountType;

                var advertisement = _context.JobAdvertisements.FirstOrDefault(x => x.Id == id);
                if (advertisement == null) return NotFound();

                if (user.AccountType == Database.Models.AccountTypes.USER)
                {
                    var advertisementModel = new JobAdvertisementDetailsUserDto(advertisement);
                    return Ok(advertisementModel);
                }
                else
                {
                    var advertisementModel = new JobAdvertisementDetailsAdminDto(advertisement);
                    return Ok(advertisementModel);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occured when processing the AdvertisementDetails request" + ex.StackTrace + "\n" + ex.Message);
                return BadRequest();
            }
          
        }

        /// <summary>
        /// Creates the new Advertisement object
        /// </summary>
        [HttpPost]
        [Route("CreateAdvertisement")]
        public async Task<IActionResult> CreateAdvertisement([FromBody] JobAdvertisementAddDto jobAdvertisementAddDto)
        {
            try
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken)) return new UnauthorizedResult();

                var user = _userAuthenticator.GetUserByToken(accessToken);

                if (user == null || user.AccountType != AccountTypes.ADMIN) return BadRequest();

                if (jobAdvertisementAddDto != null && jobAdvertisementAddDto.IsValid())
                {
                    JobAdvertisement newAdvertisement = new JobAdvertisement()
                    {
                        AdditionalInformation = jobAdvertisementAddDto.AdditionalInformation,
                        CreatedAt = DateTime.UtcNow,
                        Description = jobAdvertisementAddDto.Description,
                        EmployerId = user.Id,
                        Location = jobAdvertisementAddDto.Location,
                        MaximumNumberOfEmployees = jobAdvertisementAddDto.MaximumNumberOfEmployees,
                        Name = jobAdvertisementAddDto.Name,
                        EmployerName = user.Name,
                    };

                    _context.JobAdvertisements.Add(newAdvertisement);
                    _context.SaveChanges();

                    return Ok();
                }
                else
                    return BadRequest("The model is not valid");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured when processing the CreateAdvertisement request" + ex.StackTrace + "\n" + ex.Message);
                return BadRequest();
            }
        }


        /// <summary>
        /// Updates the specified Advertisement only if any person has not applied for this advertisement yet
        /// </summary>
        [HttpPost]
        [Route("UpdateAdvertisement")]
        public async Task<IActionResult> UpdateAdvertisement([FromBody] JobAdvertisementDetailsAdminDto jobAdvertisementDetailsAdminDto)
        {
            try
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken)) return new UnauthorizedResult();

                var user = _userAuthenticator.GetUserByToken(accessToken);

                if (user == null || user.AccountType != AccountTypes.ADMIN) return BadRequest();

                if (jobAdvertisementDetailsAdminDto != null && jobAdvertisementDetailsAdminDto.IsValid())
                {
                    var existingAdvertisement = _context.JobAdvertisements.Include(x => x.CandidateApplications).Where(x => x.Id == jobAdvertisementDetailsAdminDto.Id).FirstOrDefault();
                    if (existingAdvertisement == null) return NotFound();
                    if (existingAdvertisement.CandidateApplications.Count() > 0) return BadRequest("You can not change that offer details because at least one candidate applied for this offer");

                    existingAdvertisement.Name = jobAdvertisementDetailsAdminDto.Name;
                    existingAdvertisement.Description = jobAdvertisementDetailsAdminDto.Description;
                    existingAdvertisement.Location = jobAdvertisementDetailsAdminDto.Location;
                    existingAdvertisement.AdditionalInformation = jobAdvertisementDetailsAdminDto.AdditionalInformation;
                    existingAdvertisement.MaximumNumberOfEmployees = jobAdvertisementDetailsAdminDto.MaximumNumberOfEmployees;

                    _context.Update(existingAdvertisement);
                    _context.SaveChanges();

                    return Ok();
                }
                else
                    return BadRequest("The model is not valid");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured when processing the UpdateAdvertisement request" + ex.StackTrace + "\n" + ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Removes the Advertisement entity only if the entity belongs to the user recognized by the token
        /// </summary>
        [HttpDelete("{id}")]
        [Route("DeleteAdvertisement")]
        public async Task<IActionResult> DeleteAdvertisement([FromQuery] int id)
        {
            try
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(accessToken)) return new UnauthorizedResult();

                var user = _userAuthenticator.GetUserByToken(accessToken);

                if (user == null || user.AccountType != AccountTypes.ADMIN) return BadRequest();

                if (id > 0)
                {
                    var existingAdvertisement = _context.JobAdvertisements.Include(x => x.CandidateApplications).Where(x => x.EmployerId == user.Id && x.Id == id).FirstOrDefault();
                    if (existingAdvertisement == null) return NotFound();

                    _context.Remove(existingAdvertisement);
                    _context.SaveChanges();

                    return Ok();
                }
                else
                    return BadRequest("The given entity id has to be greater than 0");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured when processing the DeleteAdvertisement request" + ex.StackTrace + "\n" + ex.Message);
                return BadRequest();
            }
        }
    }
}
