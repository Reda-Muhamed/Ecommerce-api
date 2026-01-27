using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Products;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class AdminAttributeService : IAdminAttributeService
    {
        private readonly IAttributeRepository attributeRepository;
        private readonly IAttributeValueRepository attributeValueRepository;
        private readonly IProductRepository productRepository;
        private readonly IUnitOfWork unitOfWork;

        public AdminAttributeService(
            IAttributeRepository attributeRepository,
            IAttributeValueRepository attributeValueRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            this.attributeRepository = attributeRepository;
            this.attributeValueRepository = attributeValueRepository;
            this.productRepository = productRepository;
            this.unitOfWork = unitOfWork;
        }


        public async Task<Result<Guid>> CreateAttributeAsync(CreateAttributeDto dto, CancellationToken ct)
        {
            if (await attributeRepository.ExistsByNameAsync(dto.Name, ct))
                return Result<Guid>.Fail("AttributeAlreadyExists");

            var attribute = new ProductAttribute
            {
                Name = dto.Name.Trim(),
                Type = dto.Type,
                IsFilterable = dto.IsFilterable,
                IsVariantable = dto.IsVariantable
            };
            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                await attributeRepository.AddAsync(attribute, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<Guid>.Success(attribute.Id);

            }
            catch
            {
                await unitOfWork.RollbackAsync();
                return Result<Guid>.Fail();
            }
            
        }

        public async Task<Result<bool>> UpdateAttributeAsync(Guid attributeId, UpdateAttributeDto dto, CancellationToken ct)
        {
            var attribute = await attributeRepository.GetAsync(attributeId, ct);
            if (attribute == null)
                return Result<bool>.Fail("AttributeNotFound");

            attribute.Name = dto.Name.Trim();
            attribute.IsFilterable = dto.IsFilterable;
            attribute.IsVariantable = dto.IsVariantable;
            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                await attributeRepository.UpdateAsync(attribute, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<bool>.Success(true);

            }
            catch
            {
                await unitOfWork.RollbackAsync();
                return Result<bool>.Fail();
            }
        }


        public async Task<Result<Guid>> AddValueAsync(
            Guid attributeId,
            CreateAttributeValueDto dto,
            CancellationToken ct)
        {
            var attribute = await attributeRepository.GetAsync(attributeId, ct);
            if (attribute == null)
                return Result<Guid>.Fail("AttributeNotFound");

            if (await attributeValueRepository.ExistsAsync(attributeId, dto.Value, ct))
                return Result<Guid>.Fail("AttributeValueExists");

            var value = new AttributeValue
            {
                AttributeId = attributeId,
                Value = dto.Value.Trim(),
                SortOrder = dto.SortOrder
            };
            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                await attributeValueRepository.AddAsync(value, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<Guid>.Success(value.Id);

            }
            catch
            {
                await unitOfWork.RollbackAsync();
                return Result<Guid>.Fail();
            }
            
            
        }

        public async Task<Result<bool>> UpdateValueAsync(Guid valueId, UpdateAttributeValueDto dto, CancellationToken ct)
        {
            var value = await attributeValueRepository.GetAsync(valueId, ct);
            if (value == null)
                return Result<bool>.Fail("AttributeValueNotFound");

            value.Value = dto.Value.Trim();
            value.SortOrder = dto.SortOrder;
            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                await attributeValueRepository.UpdateAsync(value, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<bool>.Success(true);

            }
            catch
            {
                await unitOfWork.RollbackAsync();
                return Result<bool>.Fail();
            }
            
        }

        public async Task<Result<bool>> DeleteValueAsync(Guid valueId, CancellationToken ct)
        {
            var value = await attributeValueRepository.GetAsync(valueId, ct);
            if (value == null)
                return Result<bool>.Fail("AttributeValueNotFound");

            //  Safety check
            var isUsed = await productRepository.AttributeValueInUseAsync(valueId, ct);
            if (isUsed)
                return Result<bool>.Fail("AttributeValueInUse");

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                await attributeValueRepository.DeleteAsync(value, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<bool>.Success(value: true);

            }
            catch
            {
                await unitOfWork.RollbackAsync();
                return Result<bool>.Fail();
            }
        }
    }

}
