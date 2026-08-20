using Microsoft.AspNetCore.Http;
 using SafePharma.Common;
using SafePharma.DAL;
using System.Text.Json;
using UAParser;


namespace SafePharma.BLL
{
    public class AuditManager : IAuditManager
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserContext _currentUserContext;


        public AuditManager(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ICurrentUserContext currentUserContext)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _currentUserContext = currentUserContext;
        }
        public async Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllAudit()
        {

            var auditList = await _unitOfWork._auditRepository.GetAuditsWithUsers(_currentUserContext.PharmacyId);
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
        public async Task<GeneralResult<bool>> CreateAudit(object newValues, object? oldValues,string entity, ActionsEnum action)
        {
         
            if (newValues is null)
            {
                return GeneralResult<bool>.NotFound(
                    "Cannot create audit without entity data");
            }

            // 2️⃣ Action Validation
            if (!Enum.IsDefined(typeof(ActionsEnum), action))
            {
                return GeneralResult<bool>.NotFound(
                    $"Invalid action type: {action}");
            }

             if (_currentUserContext.Id == Guid.Empty)
            {
                return GeneralResult<bool>.NotFound(
                    "Cannot create audit: user context is missing");
            }

            if (_currentUserContext.PharmacyId == Guid.Empty)
            {
                return GeneralResult<bool>.NotFound(
                    "Cannot create audit: pharmacy context is missing");
            }
            if (entity == string.Empty)
            {
                return GeneralResult<bool>.NotFound(
                    "Cannot create audit: Entity context is missing");
            }

            var userAgent = _httpContextAccessor.HttpContext?
                            .Request.Headers["User-Agent"]
                            .ToString();
            var ua = Parser.GetDefault();

            var client = ua.Parse(userAgent);


            var audit = new Audit
            {
                Entity = entity,
                Action = action.ToString(),
                Date = DateTime.UtcNow,
                Device = $"{client.UA.Family} | {client.OS.Family}",
                UserId = _currentUserContext.Id,
                PharmacyId = _currentUserContext.PharmacyId,

                oldValues = oldValues is null
             ? null
             : JsonSerializer.Serialize(oldValues),

                newValues = JsonSerializer.Serialize(newValues)
            };

            _unitOfWork._auditRepository.Add(audit);


            return GeneralResult<bool>.SuccessResult(true);
        }
    }
}
