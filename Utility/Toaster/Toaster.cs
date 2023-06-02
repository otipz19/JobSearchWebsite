using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Utility.Toaster
{
    public class Toaster
    {
        public const string TempDataKey = "Toaster";

        private readonly ITempDataDictionary _tempData;

		public Toaster(ITempDataDictionary tempData)
		{
            _tempData = tempData;
		}

		public void Success(string message, string title = "Success")
        {
            SetToast(ToastType.success, title, message);
        }

        public void Info(string message, string title = "Info")
        {
            SetToast(ToastType.info, title, message);
        }

        public void Warning(string message, string title = "Warning")
        {
            SetToast(ToastType.warning, title, message);
        }

        public void Error(string message, string title = "Error")
        {
            SetToast(ToastType.error, title, message);
        }

        public void ValidationFailed(ValidationResult validationResult)
        {
            Error(validationResult.Errors.FirstOrDefault()?.ErrorMessage, "Invalid input");
        }

        private void SetToast(ToastType type, string title, string message)
        {
            _tempData.Set(TempDataKey, new Toast()
            {
                Type = type,
                Title = title,
                Message = message,
            });
        }
    }
}
