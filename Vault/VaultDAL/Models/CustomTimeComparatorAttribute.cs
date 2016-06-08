using System.ComponentModel.DataAnnotations;

namespace VaultDAL.Models
{
    public class CustomTimeComparatorAttribute:ValidationAttribute
    {
        private string _errorMessage;

        public override bool IsValid(object value)
        {
            var vault = (UserVault) value;
            var valid = false;
            if (vault.OpenTime > vault.CloseTime)
            {
                _errorMessage = "Closing time can't be less then opening";
                valid = false;
            } else if (vault.OpenTime == vault.CloseTime)
            {
                _errorMessage = "Time of closing and time of opening can't be the same";
                valid = false;
            }
            else
            {
                valid = true;
            }
            return valid;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{_errorMessage}";
        }
    }
}