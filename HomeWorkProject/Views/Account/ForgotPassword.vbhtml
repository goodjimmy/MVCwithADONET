﻿@ModelType ForgotPasswordViewModel
@Code
    ViewBag.Title = "忘記密碼?"
End Code

<h2>@ViewBag.Title.</h2>

@Using Html.BeginForm("ForgotPassword", "Account", FormMethod.Post, New With {.class = "form-horizontal", .role = "form"})
    @Html.AntiForgeryToken()
    @<text>
    <h4>輸入您的電子郵件。</h4>
    <hr />
    @Html.ValidationSummary("", New With {.class = "text-danger"})
    <div class="form-group">
        @Html.LabelFor(Function(m) m.Email, New With {.class = "col-md-2 control-label"})
        <div class="col-md-10">
            @Html.TextBoxFor(Function(m) m.Email, New With {.class = "form-control"})
        </div>
    </div>
    <div class="form-group">
        <div class="col-md-offset-2 col-md-10">
            <input type="submit" class="btn btn-default" value="電子郵件連結" />
        </div>
    </div>
    </text>
End Using

@section Scripts
    @Scripts.Render("~/bundles/jqueryval")
End Section
