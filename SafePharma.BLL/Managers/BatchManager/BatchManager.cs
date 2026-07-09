using SafePharma.Common;
using SafePharma.Common.Enums;
using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UAParser;

namespace SafePharma.BLL
{
    public class BatchManager
    {
        public IUnitOfWork _unitOfWork;

        public BatchManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<IEnumerable<AuditReadDto>>> GetAllBatches()
        {

            var auditList = await _unitOfWork._auditRepository.GetAuditsWithUsers();
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

        public async Task<GeneralResult<BatchCreateDto>> CreateBatch(BatchCreateDto batchDto)
        {
            if (batchDto is null)
            {
                GeneralResult<BatchCreateDto>.NotFound();
            }
            // get medecine form medicine manager
            var batch = new Batch
            {
               MedicineId= batchDto.MedicineId,
               BatchNumber= batchDto.BatchNumber,
               ExpiryDate= batchDto.ExpiryDate,
               QuantityReceived= batchDto.QuantityReceived,
               QuantityRemaining=batchDto.QuantityReceived,
               //assign medicine prices 
               //selling
               //purchased
          
            };
            _unitOfWork._batchRepository.Add(batch);
            await _unitOfWork.SaveAsync();

            return GeneralResult<BatchCreateDto>.SuccessResult(batchDto);
        }

    }
}
