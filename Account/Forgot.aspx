<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Forgot.aspx.vb" Inherits="Account_Forgot" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Forgot Password - BOOS</title>
    <link href="https://fonts.googleapis.com/css2?family=Nunito:wght@300;400;600;700;800&display=swap" rel="stylesheet">

    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/bootstrap.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/vendors/bootstrap-icons/bootstrap-icons.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/app.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/pages/auth.css") %>" />
</head>

<body>
    <form id="formForgot" runat="server">
        <div id="auth">
            <div class="row h-100">
                <div class="col-lg-5 col-12">
                    <div id="auth-left">
                        <h1 class="auth-title">Forgot Password</h1>
                        <p class="auth-subtitle mb-3">Input your user and email and we will send you new password.</p>
                        <div class="form-group position-relative has-icon-left mb-3">
                            <asp:TextBox runat="server" ID="txtUserLogin" CssClass="form-control form-control-xl" placeholder="User Name" autocomplete="off"></asp:TextBox>
                            <div class="form-control-icon">
                                <i class="bi bi-person"></i>
                            </div>
                        </div>

                        <div class="form-group position-relative has-icon-left">
                            <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control form-control-xl" placeholder="Your Email" autocomplete="off"></asp:TextBox>
                            <div class="form-control-icon">
                                <i class="bi bi-envelope"></i>
                            </div>
                        </div>

                        <div class="alert alert-danger mt-2" runat="server" id="divError">
                            <span runat="server" id="msgError"></span>
                        </div>

                        <asp:Button runat="server" ID="btnSend" CssClass="btn btn-primary btn-block btn-lg shadow-lg mt-5" Text="Send" OnClick="btnSend_Click" />

                        <div class="text-center mt-3 text-lg fs-4">
                            <p class='text-gray-600'>
                                Remember your account?
                                <a runat="server" href="~/account/login" class="font-bold">Log in</a>.
                            </p>
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 d-none d-lg-block">
                    <div id="auth-right"></div>
                </div>
            </div>
        </div>
    </form>
</body>

</html>
