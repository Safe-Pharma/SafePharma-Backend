using Microsoft.AspNetCore.Http;
using SafePharma.BLL.DTOs.Audit;
using SafePharma.Common;
using SafePharma.Common;
using SafePharma.DAL;
using System.Text.Json;
using UAParser;


namespace SafePharma.BLL
{
    public class AuditManager : IAuditManager
    {
        public IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserContext _currentUserContext;


        public AuditManager(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ICurrentUserContext currentUserContext)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _currentUserContext = currentUserContext;
        }
        public async Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllAudit(Guid pharmacyId)
        {

            var auditList = await _unitOfWork._auditRepository.GetAuditsWithUsers(pharmacyId);
            IEnumerable<AuditReadDto> auditReadList = auditList.Select(a => new AuditReadDto
            {
                Entity = a.Entity,
                Action = a.Action,
                Date = a.Date,
                Device = a.Device,
                UserFullName = a.User.UserName!,
                oldValues = string.IsNullOrWhiteSpace(a.oldValues)
                            ? null
                            : JsonSerializer.Deserialize<JsonElement>(a.oldValues),
                newValues = string.IsNullOrWhiteSpace(a.newValues)
                            ? null
                            : JsonSerializer.Deserialize<JsonElement>(a.newValues)
            }).ToList();
            return GeneralResult<IEnumerable<AuditReadDto>>.SuccessResult(auditReadList);
        }
        public async Task<GeneralResult<AuditCreateDto>> CreateAudit(object newValues, object? oldValues, ActionsEnum action)
        {
            if(newValues is null)
            {
                GeneralResult<AuditCreateDto>.FailResult();
            }

            var userAgent = _httpContextAccessor.HttpContext?
                            .Request.Headers["User-Agent"]
                            .ToString();
            var ua = Parser.GetDefault();

            var client = ua.Parse(userAgent);


            var auditDto = new AuditCreateDto
            {
                Device = $"{client.UA.Family} | {client.OS.Family}",
                Action = action,
                Entity = newValues.GetType().Name.ToString() ?? string.Empty,
                newValues = JsonSerializer.Serialize(newValues) ?? "",
                UserId = _currentUserContext.Id,
                oldValues = JsonSerializer.Serialize(oldValues) ?? "",
                Date= DateTime.Now,
            };
            var auditUser = await _unitOfWork._auditRepository.GetAuditWithUserId(auditDto.UserId);
            if (auditUser == null)
            {
                return GeneralResult<AuditCreateDto>.NotFound();
            }

            Audit newAudit = new Audit
            {
                Entity = auditDto.Entity,
                Action = auditDto.Action.ToString(),
                Date = auditDto.Date,
                Device = auditDto.Device,
                UserId = auditDto.UserId,
                User = auditUser.User,
                oldValues = auditDto.oldValues,
                newValues = auditDto.newValues,
            };
           
            _unitOfWork._auditRepository.Add(newAudit);
            await _unitOfWork.SaveAsync();

            return GeneralResult<AuditCreateDto>.SuccessResult(auditDto);
        }
    }
}
