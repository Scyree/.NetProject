﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Domain.Interfaces.ServicesInterfaces
{
    public interface ILectureService
    {
        List<string> GetFiles(Guid id);
        List<string> GetFilesBasedOnDetails(string title, string description);
    }
}
