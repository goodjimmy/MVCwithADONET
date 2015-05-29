@Code
    ViewBag.Title = "ConfirmAccount"
End Code

<h2>@ViewBag.Title.</h2>
<div>
    <p>
        感謝您確認您的帳戶。請 @Html.ActionLink("按一下這裡以登入", "Login", "Account", routeValues:=Nothing, htmlAttributes:=New With {Key .id = "loginLink"})
    </p>
</div>
