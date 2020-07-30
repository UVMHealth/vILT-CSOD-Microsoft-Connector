using System;

namespace vILT.Domain
{
    public static class ViltErrorExtensions
    {
        public static ViltConnectorErrorResponse ToViltError(this Exception exc)
        {
            var viltError = new ViltConnectorError
            {
                Code = 500
            };
            if (exc.InnerException == null || string.IsNullOrEmpty(exc.InnerException?.Message))
            {
                viltError.Message = exc.Message;
            }
            else
            {
                viltError.Message = $"Exception: {exc.Message}. InnerException: {exc.InnerException.Message}";
            }
            return new ViltConnectorErrorResponse { Error = viltError, Timestamp = DateTimeOffset.Now.ToString() };

        }
    }
}
