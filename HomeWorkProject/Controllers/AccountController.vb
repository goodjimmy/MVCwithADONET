Imports System.Security.Claims
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin.Security
Imports Owin

<Authorize>
Public Class AccountController
    Inherits Controller

    Private _userManager As ApplicationUserManager

    Public Sub New()
    End Sub

    Public Sub New(manager As ApplicationUserManager)
        UserManager = manager
    End Sub

    Public Property UserManager() As ApplicationUserManager
        Get
            Return If(_userManager, HttpContext.GetOwinContext().GetUserManager(Of ApplicationUserManager)())
        End Get
        Private Set(value As ApplicationUserManager)
            _userManager = value
        End Set
    End Property

    '
    ' GET: /Account/Login
    <AllowAnonymous>
    Public Function Login(returnUrl As String) As ActionResult
        ViewBag.ReturnUrl = returnUrl
        Return View()
    End Function

    '
    ' POST: /Account/Login
    <HttpPost>
    <AllowAnonymous>
    <ValidateAntiForgeryToken>
    Public Async Function Login(model As LoginViewModel, returnUrl As String) As Task(Of ActionResult)
        If ModelState.IsValid Then
            ' 驗證密碼
            Dim appUser = Await UserManager.FindAsync(model.Email, model.Password)
            If appUser IsNot Nothing Then
                Await SignInAsync(appUser, model.RememberMe)
                Return RedirectToLocal(returnUrl)
            Else
                ModelState.AddModelError("", "無效的使用者名稱和密碼。")
            End If
        End If

        ' 如果執行到這裡，發生某項失敗，則重新顯示表單
        Return View(model)
    End Function

    '
    ' GET: /Account/Register
    <AllowAnonymous>
    Public Function Register() As ActionResult
        Return View()
    End Function

    '
    ' POST: /Account/Register
    <HttpPost>
    <AllowAnonymous>
    <ValidateAntiForgeryToken>
    Public Async Function Register(model As RegisterViewModel) As Task(Of ActionResult)
        If ModelState.IsValid Then
            ' 登入使用者前請先建立本機登入
            Dim user = New ApplicationUser() With {.UserName = model.Email, .Email = model.Email}
            Dim result = Await UserManager.CreateAsync(User, model.Password)
            If result.Succeeded Then
                Await SignInAsync(User, isPersistent:=False)

                ' 如需如何啟用帳戶確認和密碼重設的詳細資訊，請造訪 http://go.microsoft.com/fwlink/?LinkID=320771
                ' 傳送包含此連結的電子郵件
                ' Dim code = Await UserManager.GenerateEmailConfirmationTokenAsync(user.Id)
                ' Dim callbackUrl = Url.Action("ConfirmEmail", "Account", New With {.code = code, .userId = user.Id}, protocol:=Request.Url.Scheme)
                ' Await UserManager.SendEmailAsync(user.Id, "確認您的帳戶", "請按一下此連結確認您的帳戶 <a href=""" & callbackUrl & """>這裏</a>")

                Return RedirectToAction("Index", "Home")
            Else
                AddErrors(result)
            End If
        End If

        ' 如果執行到這裡，發生某項失敗，則重新顯示表單
        Return View(model)
    End Function

    '
    ' GET: /Account/ConfirmEmail
    <AllowAnonymous>
    Public Async Function ConfirmEmail(userId As String, code As String) As Task(Of ActionResult)
        If userId Is Nothing OrElse code Is Nothing Then
            Return View("Error")
        End If

        Dim result = Await UserManager.ConfirmEmailAsync(userId, code)
        If result.Succeeded Then
            Return View("ConfirmEmail")
        Else
            AddErrors(result)
            Return View()
        End If
    End Function

    '
    ' GET: /Account/ForgotPassword
    <AllowAnonymous>
    Public Function ForgotPassword() As ActionResult
        Return View()
    End Function

    '
    ' POST: /Account/ForgotPassword
    <HttpPost>
    <AllowAnonymous>
    <ValidateAntiForgeryToken>
    Public Async Function ForgotPassword(model As ForgotPasswordViewModel) As Task(Of ActionResult)
        If ModelState.IsValid Then
            Dim user = Await UserManager.FindByNameAsync(model.Email)
            If user Is Nothing OrElse Not (Await UserManager.IsEmailConfirmedAsync(user.Id)) Then
                ModelState.AddModelError("", "使用者不存在或未確認。")
                Return View()
            End If

            ' 如需如何啟用帳戶確認和密碼重設的詳細資訊，請造訪 http://go.microsoft.com/fwlink/?LinkID=320771
            ' 傳送包含此連結的電子郵件
            ' Dim code As String = Await UserManager.GeneratePasswordResetTokenAsync(user.Id)
            ' Dim callbackUrl = Url.Action("ResetPassword", "Account", New With {.code = code, .userId = user.Id}, protocol:=Request.Url.Scheme)
            ' Await UserManager.SendEmailAsync(user.Email, "重設密碼", "請按 <a href=""" & callbackUrl & """>這裡</a>")
            ' Return RedirectToAction("ForgotPasswordConfirmation", "Account")
        End If

        ' 如果執行到這裡，發生某項失敗，則重新顯示表單
        Return View(model)
    End Function

    '
    ' GET: /Account/ForgotPasswordConfirmation
    <AllowAnonymous>
    Public Function ForgotPasswordConfirmation() As ActionResult
        Return View()
    End Function

    '
    ' GET: /Account/ResetPassword
    <AllowAnonymous>
    Public Function ResetPassword(code As String) As ActionResult
        If code Is Nothing Then
            Return View("Error")
        End If
        Return View()
    End Function

    '
    ' POST: /Account/ResetPassword
    <HttpPost>
    <AllowAnonymous>
    <ValidateAntiForgeryToken>
    Public Async Function ResetPassword(model As ResetPasswordViewModel) As Task(Of ActionResult)
        If ModelState.IsValid Then
            Dim user = Await UserManager.FindByNameAsync(model.Email)
            If user Is Nothing Then
                ModelState.AddModelError("", "找不到使用者。")
                Return View()
            End If
            Dim result = Await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password)
            If result.Succeeded Then
                Return RedirectToAction("ResetPasswordConfirmation", "Account")
            Else
                AddErrors(result)
                Return View()
            End If
        End If

        ' 如果執行到這裡，發生某項失敗，則重新顯示表單
        Return View(model)
    End Function

    '
    ' GET: /Account/ResetPasswordConfirmation
    <AllowAnonymous>
    Public Function ResetPasswordConfirmation() As ActionResult
        Return View()
    End Function

    '
    ' POST: /Account/Disassociate
    <HttpPost>
    <ValidateAntiForgeryToken>
    Public Async Function Disassociate(loginProvider As String, providerKey As String) As Task(Of ActionResult)
        Dim message As ManageMessageId? = Nothing
        Dim result As IdentityResult = Await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), New UserLoginInfo(loginProvider, providerKey))
        If result.Succeeded Then
            Dim userInfo = Await UserManager.FindByIdAsync(User.Identity.GetUserId())
            Await SignInAsync(userInfo, isPersistent:=False)
            message = ManageMessageId.RemoveLoginSuccess
        Else
            message = ManageMessageId.Error
        End If

        Return RedirectToAction("Manage", New With {
            .Message = message
        })
    End Function

    '
    ' GET: /Account/Manage
    Public Function Manage(ByVal message As ManageMessageId?) As ActionResult
        ViewData("StatusMessage") =
            If(message = ManageMessageId.ChangePasswordSuccess, "您的密碼已變更。",
                If(message = ManageMessageId.SetPasswordSuccess, "已設定您的密碼。",
                    If(message = ManageMessageId.RemoveLoginSuccess, "已移除外部登入。",
                        If(message = ManageMessageId.Error, "發生錯誤。",
                        ""))))
        ViewBag.HasLocalPassword = HasPassword()
        ViewBag.ReturnUrl = Url.Action("Manage")
        Return View()
    End Function

    '
    ' POST: /Account/Manage
    <HttpPost>
    <ValidateAntiForgeryToken>
    Public Async Function Manage(model As ManageUserViewModel) As Task(Of ActionResult)
        Dim hasLocalLogin As Boolean = HasPassword()
        ViewBag.HasLocalPassword = hasLocalLogin
        ViewBag.ReturnUrl = Url.Action("Manage")
        If hasLocalLogin Then
            If ModelState.IsValid Then
                Dim result As IdentityResult = Await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword)
                If result.Succeeded Then
                    Dim userInfo = Await UserManager.FindByIdAsync(User.Identity.GetUserId())
                    Await SignInAsync(userInfo, isPersistent:=False)
                    Return RedirectToAction("Manage", New With {
                        .Message = ManageMessageId.ChangePasswordSuccess
                    })
                Else
                    AddErrors(result)
                End If
            End If
        Else
            ' 使用者沒有本機密碼，因此，請移除遺漏 OldPassword 欄位所導致的任何驗證錯誤
            Dim state As ModelState = ModelState("OldPassword")
            If state IsNot Nothing Then
                state.Errors.Clear()
            End If

            If ModelState.IsValid Then
                Dim result As IdentityResult = Await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword)
                If result.Succeeded Then
                    Return RedirectToAction("Manage", New With {
                        .Message = ManageMessageId.SetPasswordSuccess
                    })
                Else
                    AddErrors(result)
                End If
            End If
        End If

        ' 如果執行到這裡，發生某項失敗，則重新顯示表單
        Return View(model)
    End Function

    '
    ' POST: /Account/ExternalLogin
    <HttpPost>
    <AllowAnonymous>
    <ValidateAntiForgeryToken>
    Public Function ExternalLogin(provider As String, returnUrl As String) As ActionResult
        ' 要求重新導向至外部登入提供者
        Return New ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", New With {.ReturnUrl = returnUrl}))
    End Function

    '
    ' GET: /Account/ExternalLoginCallback
    <AllowAnonymous>
    Public Async Function ExternalLoginCallback(returnUrl As String) As Task(Of ActionResult)
        Dim loginInfo = Await AuthenticationManager.GetExternalLoginInfoAsync()
        If loginInfo Is Nothing Then
            Return RedirectToAction("Login")
        End If

        ' 若使用者已經有登入資料，請使用此外部登入提供者登入使用者
        Dim user = Await UserManager.FindAsync(loginInfo.Login)
        If user IsNot Nothing Then
            Await SignInAsync(user, isPersistent:=False)
            Return RedirectToLocal(returnUrl)
        Else
            ' 若使用者沒有帳戶，請提示使用者建立帳戶
            ViewBag.ReturnUrl = returnUrl
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider
            Return View("ExternalLoginConfirmation", New ExternalLoginConfirmationViewModel() With {.Email = loginInfo.Email})
        End If
        Return View("ExternalLoginFailure")
    End Function

    '
    ' POST: /Account/LinkLogin
    <HttpPost>
    <ValidateAntiForgeryToken>
    Public Function LinkLogin(provider As String) As ActionResult
        ' 要求重新導向至外部登入提供者以連結目前使用者的登入
        Return New ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId())
    End Function

    '
    ' GET: /Account/LinkLoginCallback
    Public Async Function LinkLoginCallback() As Task(Of ActionResult)
        Dim loginInfo = Await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId())
        If loginInfo Is Nothing Then
            Return RedirectToAction("Manage", New With {
                .Message = ManageMessageId.Error
            })
        End If
        Dim result = Await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login)
        If result.Succeeded Then
            Return RedirectToAction("Manage")
        End If
        Return RedirectToAction("Manage", New With {
            .Message = ManageMessageId.Error
        })
    End Function

    '
    ' POST: /Account/ExternalLoginConfirmation
    <HttpPost>
    <AllowAnonymous>
    <ValidateAntiForgeryToken>
    Public Async Function ExternalLoginConfirmation(model As ExternalLoginConfirmationViewModel, returnUrl As String) As Task(Of ActionResult)
        If User.Identity.IsAuthenticated Then
            Return RedirectToAction("Manage")
        End If

        If ModelState.IsValid Then
            ' 從外部登入提供者處取得使用者資訊
            Dim info = Await AuthenticationManager.GetExternalLoginInfoAsync()
            If info Is Nothing Then
                Return View("ExternalLoginFailure")
            End If
            Dim user = New ApplicationUser() With {.UserName = model.Email, .Email = model.Email}
            Dim result = Await UserManager.CreateAsync(user)
            If result.Succeeded Then
                result = Await UserManager.AddLoginAsync(user.Id, info.Login)
                If result.Succeeded Then
                    Await SignInAsync(user, isPersistent:=False)

                    ' 如需如何啟用帳戶確認和密碼重設的詳細資訊，請造訪  http://go.microsoft.com/fwlink/?LinkID=320771
                    ' 傳送包含此連結的電子郵件
                    ' Dim code = Await UserManager.GenerateEmailConfirmationTokenAsync(user.Id)
                    ' Dim callbackUrl = Url.Action("ConfirmEmail", "Account", New With { .code = code, .userId = user.Id }, protocol := Request.Url.Scheme)
                    ' SendEmail(user.Email, callbackUrl, "確認您的帳戶", "請按一下此連結確認您的帳戶")

                    Return RedirectToLocal(returnUrl)
                End If
            End If
            AddErrors(result)
        End If

        ViewBag.ReturnUrl = returnUrl
        Return View(model)
    End Function

    '
    ' POST: /Account/LogOff
    <HttpPost>
    <ValidateAntiForgeryToken>
    Public Function LogOff() As ActionResult
        AuthenticationManager.SignOut()
        Return RedirectToAction("Index", "Home")
    End Function

    '
    ' GET: /Account/ExternalLoginFailure
    <AllowAnonymous>
    Public Function ExternalLoginFailure() As ActionResult
        Return View()
    End Function

    <ChildActionOnly>
    Public Function RemoveAccountList() As ActionResult
        Dim linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId())
        ViewBag.ShowRemoveButton = linkedAccounts.Count > 1 Or HasPassword()
        Return DirectCast(PartialView("_RemoveAccountPartial", linkedAccounts), ActionResult)
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso UserManager IsNot Nothing Then
            UserManager.Dispose()
            UserManager = Nothing
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Helper"
    ' 新增外部登入時用來當做 XSRF 保護
    Private Const XsrfKey As String = "XsrfId"

    Private Function AuthenticationManager() As IAuthenticationManager
        Return HttpContext.GetOwinContext().Authentication
    End Function

    Private Async Function SignInAsync(user As ApplicationUser, isPersistent As Boolean) As Task
        AuthenticationManager.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie)
        AuthenticationManager.SignIn(New AuthenticationProperties() With {.IsPersistent = isPersistent}, Await user.GenerateUserIdentityAsync(UserManager))
    End Function

    Private Sub AddErrors(result As IdentityResult)
        For Each [error] As String In result.Errors
            ModelState.AddModelError("", [error])
        Next
    End Sub

    Private Function HasPassword() As Boolean
        Dim appUser = UserManager.FindById(User.Identity.GetUserId())
        If (appUser IsNot Nothing) Then
            Return appUser.PasswordHash IsNot Nothing
        End If
        Return False
    End Function

    Private Sub SendEmail(email As String, callbackUrl As String, subject As String, message As String)
        ' 如需傳送郵件的詳細資訊，請造訪 http://go.microsoft.com/fwlink/?LinkID=320771
    End Sub

    Private Function RedirectToLocal(returnUrl As String) As ActionResult
        If Url.IsLocalUrl(returnUrl) Then
            Return Redirect(returnUrl)
        Else
            Return RedirectToAction("Index", "Home")
        End If
    End Function

    Public Enum ManageMessageId
        ChangePasswordSuccess
        SetPasswordSuccess
        RemoveLoginSuccess
        [Error]
    End Enum

    Private Class ChallengeResult
        Inherits HttpUnauthorizedResult
        Public Sub New(provider As String, redirectUri As String)
            Me.New(provider, redirectUri, Nothing)
        End Sub
        Public Sub New(provider As String, redirectUri As String, userId As String)
            Me.LoginProvider = provider
            Me.RedirectUri = redirectUri
            Me.UserId = userId
        End Sub

        Public Property LoginProvider As String
        Public Property RedirectUri As String
        Public Property UserId As String

        Public Overrides Sub ExecuteResult(context As ControllerContext)
            Dim properties = New AuthenticationProperties() With {.RedirectUri = RedirectUri}
            If UserId IsNot Nothing Then
                properties.Dictionary(XsrfKey) = UserId
            End If
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider)
        End Sub
    End Class
#End Region

End Class
