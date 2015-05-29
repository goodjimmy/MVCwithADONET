Imports System.ComponentModel.DataAnnotations

Public Class ExternalLoginConfirmationViewModel
    <Required>
    <EmailAddress> _
    <Display(Name:="電子郵件")>
    Public Property Email As String
End Class

Public Class ExternalLoginListViewModel
    Public Property Action As String
    Public Property ReturnUrl As String
End Class

Public Class ManageUserViewModel
    <Required>
    <DataType(DataType.Password)>
    <Display(Name:="目前密碼")>
    Public Property OldPassword As String

    <Required>
    <StringLength(100, ErrorMessage:="{0} 的長度至少必須為 {2} 個字元。", MinimumLength:=6)>
    <DataType(DataType.Password)>
    <Display(Name:="新密碼")>
    Public Property NewPassword As String

    <DataType(DataType.Password)>
    <Display(Name:="確認新密碼")>
    <Compare("NewPassword", ErrorMessage:="新密碼與確認密碼不相符。")>
    Public Property ConfirmPassword As String
End Class

Public Class LoginViewModel
    <Required>
    <EmailAddress> _
    <Display(Name:="電子郵件")>
    Public Property Email As String

    <Required>
    <DataType(DataType.Password)>
    <Display(Name:="密碼")>
    Public Property Password As String

    <Display(Name:="記住我?")>
    Public Property RememberMe As Boolean
End Class

Public Class RegisterViewModel
    <Required>
    <EmailAddress> _
    <Display(Name:="電子郵件")>
    Public Property Email As String

    <Required>
    <StringLength(100, ErrorMessage:="{0} 的長度至少必須為 {2} 個字元。", MinimumLength:=6)>
    <DataType(DataType.Password)>
    <Display(Name:="密碼")>
    Public Property Password As String

    <DataType(DataType.Password)>
    <Display(Name:="確認密碼")>
    <Compare("Password", ErrorMessage:="密碼和確認密碼不相符。")>
    Public Property ConfirmPassword As String
End Class

Public Class ResetPasswordViewModel
    <Required> _
    <EmailAddress> _
    <Display(Name:="電子郵件")> _
    Public Property Email() As String

    <Required> _
    <StringLength(100, ErrorMessage:="{0} 的長度至少必須為 {2} 個字元。", MinimumLength:=6)> _
    <DataType(DataType.Password)> _
    <Display(Name:="密碼")> _
    Public Property Password() As String

    <DataType(DataType.Password)> _
    <Display(Name:="確認密碼")> _
    <Compare("Password", ErrorMessage:="密碼和確認密碼不相符。")> _
    Public Property ConfirmPassword() As String

    Public Property Code() As String
End Class

Public Class ForgotPasswordViewModel
    <Required> _
    <EmailAddress> _
    <Display(Name:="電子郵件")> _
    Public Property Email() As String
End Class
