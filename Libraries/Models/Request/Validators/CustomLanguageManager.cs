using FluentValidation.Resources;

namespace Models.Request.Validators
{
    public class CustomLanguageManager : LanguageManager
    {
        public CustomLanguageManager()
        {
            AddTranslation("zh-tw", "NotEmptyValidator", "'{PropertyNamte}' 不可為空");
            AddTranslation("zh-tw", "NotNullValidator", "'{PropertyNamte}' 為必填欄位");
        }
    }
}
