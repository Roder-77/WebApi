using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Models.Attributes
{
    public class CustomRequired : RequiredAttribute
    {
        public CustomRequired([CallerMemberName] string memberName = "")
        {
            ErrorMessage = $"{memberName} 為必填";
        }
    }

    public class CustomStringLength : StringLengthAttribute
    {
        public CustomStringLength(int maximumLength, [CallerMemberName] string memberName = "") : base(maximumLength)
        {
            ErrorMessage = $"{memberName} 不可小於 {MinimumLength} & 不可大於 {maximumLength}";
        }
    }

    public class DecimalValidation : RegularExpressionAttribute
    {
        public DecimalValidation() : base(@"^\$?\d+(\.(\d{1,3}))?$")
        { }
    }
}
