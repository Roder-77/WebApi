using FluentValidation.Resources;

namespace Models.Requests.Validators
{
    public class CustomLanguageManager : LanguageManager
    {
        public CustomLanguageManager()
        {
            AddTranslation("zh-tw", "NotEmptyValidator", "'{PropertyName}' 不可為空");
            AddTranslation("zh-tw", "NotNullValidator", "'{PropertyName}' 為必填欄位");
        }
    }
}
