using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Enums;
using OnlineDemonstrator.Libraries.Domain.Models;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Implementations
{
    public class DeviceService : IDeviceService
    {
        private readonly IContextFactory<ApplicationContext> _contextFactory;
        private readonly IStringLocalizer<AppResources> _stringLocalizer;

        public DeviceService(IContextFactory<ApplicationContext> contextFactory,
            IStringLocalizer<AppResources> stringLocalizer)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        public async Task<BaseResult> AddAsync([FromBody, Required] DeviceIn deviceIn)
        {
            if (deviceIn == null) throw new ArgumentNullException(nameof(deviceIn));

            await using var context = _contextFactory.CreateContext();
            var isValidBaseDevice = Enum.TryParse(deviceIn.BaseOs, out OperationSystems baseOs);

            var targetDevice = await context.Devices.FirstOrDefaultAsync(x => x.Id == deviceIn.DeviceId);

            if (targetDevice != null)
            {
                targetDevice.FcmToken = deviceIn.FcmToken;
                targetDevice.LastVisitDate = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
            else
            {
                var device = new Device
                {
                    Id = deviceIn.DeviceId,
                    CreatedDate = DateTime.UtcNow,
                    LastVisitDate = DateTime.UtcNow,
                    FcmToken = deviceIn.FcmToken,
                    OsId = isValidBaseDevice ? (int)baseOs : (int)OperationSystems.Unknown
                };
                await context.Devices.AddAsync(device);
                await context.SaveChangesAsync();  
            }
            return new BaseResult();
        }

        public async Task<Device> GetAsync([FromBody, Required] DeviceIn deviceIn)
        {
            if (deviceIn == null) throw new ArgumentNullException(nameof(deviceIn));

            await using var context = _contextFactory.CreateContext();

            var targetDevice = await context.Devices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == deviceIn.DeviceId);

            return targetDevice;
        }
    }
}