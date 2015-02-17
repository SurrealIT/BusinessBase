using System;
using System.Collections.Generic;
using GamePlan.Core.Models;

namespace GamePlan.Core.Services.ClientSettings
{
    public interface IClientSettingsService
    {
        List<IClientSettingsModel> ClientSettingsModels(bool autoSave = false);

        IClientSettingsModel GetClientSettingsModel(Guid id, bool autoSave = false);
    }
}