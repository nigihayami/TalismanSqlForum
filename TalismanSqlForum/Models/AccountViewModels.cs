using System.ComponentModel.DataAnnotations;

namespace TalismanSqlForum.Models
{
    public abstract class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ManageUserViewModel
    {
        public ManageUserViewModel(string confirmPassword, string newPassword, string oldPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
        }

        [Required(ErrorMessage = "Требуется поле Текущий пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; private set; }

        [Required(ErrorMessage = "Требуется поле Новый пароль")]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; private set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; private set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Требуется поле Адрес электронной почты")]
        [EmailAddress(ErrorMessage = "Неверный формат адреса электронной почты")]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage="Требуется поле Пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        #region Основная информация
        [Required(ErrorMessage = "Требуется поле Адрес электронной почты")]
        [EmailAddress(ErrorMessage="Неверный формат адреса электронной почты")]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Требуется поле Пароль")]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Требуется поле Имя пользователя")]
        [Display(Name = "Имя пользователя")]
        public string NickName { get; set; }
        #endregion
        #region Доп информация
        [Required(ErrorMessage = "Требуется поле Полное наименование учреждения")]
        [Display(Name = "Полное наименование учреждения")]
        public string Name_Org { get; set; }
        [Required(ErrorMessage = "Требуется поле Краткое наименование учреждения")]
        [Display(Name = "Краткое наименование учреждения")]
        public string Mnemo_Org { get; set; }
        [Required(ErrorMessage = "Требуется поле ИНН")]
        [Display(Name = "ИНН")]
        [StringLength(12, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 10)]
        public string Inn { get; set; }
        [Required(ErrorMessage = "Требуется поле Адрес")]
        [Display(Name = "Адрес")]
        public string Adres { get; set; }
        [Display(Name = "Контактное лицо")]
        public string Contact_Name { get; set; }
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        #endregion

    }
}
