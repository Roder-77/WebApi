using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
        public DecimalValidation(int decimalPointPlace = 0) : base(decimalPointPlace <= 0 ? @"^\d+$" : $@"^\d+(\.(\d{{1,{decimalPointPlace}}}))?$")
        {
            ErrorMessage = decimalPointPlace <= 0 ? "不可輸入小數點" : $"僅包含到小數點後 {decimalPointPlace} 位";
        }
    }

    public class PositiveNumberValidation : RegularExpressionAttribute
    {
        public PositiveNumberValidation() : base(@"^\d+$")
        {
            ErrorMessage = "僅可輸入正整數";
        }
    }

    public class YoutubeUrlValidation : RegularExpressionAttribute
    {
        public YoutubeUrlValidation() : base(@"^(https?:\/\/)?(www\.)?(youtube\.com\/(watch\?v=|embed\/|v\/)|youtu\.be\/)([a-zA-Z0-9_-]{11})(\S+)?$")
        {
            ErrorMessage = "請輸入正確 Youtube 連結";
        }
    }

    public class RouteNameValidation : RegularExpressionAttribute
    {
        public RouteNameValidation() : base(@"^[a-zA-Z0-9_-]+$")
        {
            ErrorMessage = "僅可輸入英文、數字、底線、中線";
        }
    }

    public class PhoneValidation : RegularExpressionAttribute
    {
        public PhoneValidation() : base(@"^[0-9\+\# -]+$")
        {
            ErrorMessage = "僅可輸入數字、加號、中線、空格或井字鍵";
        }
    }
}
