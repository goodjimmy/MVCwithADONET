@Code
    ViewBag.Title = "重設密碼確認"
End Code

<hgroup class="title">
    <h1>@ViewBag.Title.</h1>
</hgroup>
<div>
    <p>
        您的密碼已重設。請 @Html.ActionLink("按一下這裡登入", "Login", "Account", routeValues:=Nothing, htmlAttributes:=New With {Key .id = "loginLink"})
    </p>
</div>
