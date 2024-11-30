using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryOfPreferences.Application.Implementations.Base
{
    public class BaseService
    {
        public static string FormatFullNotFoundErrorMessage(Guid id, string nameOfEntity)
            => $"The {nameOfEntity} with Id {id} has not been found.";
        public static string FormatBadRequestErrorMessage(Guid id, string nameOfEntity)
            => $"The {nameOfEntity} with id: {id} is not active.";
        public static string BadRequestEntityExistsMessage(Guid id, string nameOfEntity)
       => $"The {nameOfEntity} with id:{id} already exists.";
    }
}
