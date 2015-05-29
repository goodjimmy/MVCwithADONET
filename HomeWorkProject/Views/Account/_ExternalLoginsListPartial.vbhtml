@ModelType ExternalLoginListViewModel
@Imports Microsoft.Owin.Security
@Code
    Dim loginProviders = Context.GetOwinContext().Authentication.GetExternalAuthenticationTypes()
End Code
<h4>使用其他服務登入。</h4>
<hr />
@If loginProviders.Count() = 0 Then
    @<div>
        <p>
            沒有已設定的外部驗證服務。請參閱<a href="http://go.microsoft.com/fwlink/?LinkId=313242">本文</a>
            以進一步瞭解如何設定此 ASP.NET 應用程式以支援透過外部服務的登入。
        </p>
    </div>
Else
    @Using Html.BeginForm(Model.Action, "Account", New With {.ReturnUrl = Model.ReturnUrl}, FormMethod.Post, New With {.class = "form-horizontal", .role = "form"})
        @Html.AntiForgeryToken()
        @<div id="socialLoginList">
           <p>
               @For Each p As AuthenticationDescription In loginProviders
                   @<button type="submit" class="btn btn-default" id="@p.AuthenticationType" name="provider" value="@p.AuthenticationType" title="使用 @p.Caption 帳戶登入">@p.AuthenticationType</button>
               Next
           </p>
        </div>
    End Using
End If
