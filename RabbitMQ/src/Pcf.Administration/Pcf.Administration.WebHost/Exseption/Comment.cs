using System;

namespace Pcf.Administration.WebHost.Exseption
{
    public static class Comment
    {
        public static string FormatNotFoundErrorMessage(Guid id, string nameOfEntity)
            => $"The {nameOfEntity} with Id {id} has not been found.";
    }
}
